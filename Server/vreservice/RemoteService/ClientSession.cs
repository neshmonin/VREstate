using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;

namespace Vre.Server.RemoteService
{
    internal class ClientSessionStore : IDisposable
    {
        private Dictionary<string, ClientSession> _sessionList;
        private ManualResetEvent _staleSessionDropThreadExit;
        private int _cleanupTimeoutSec;
        private int _dbSessionTimeoutSec;

        public ClientSessionStore()
        {
            _sessionList = new Dictionary<string, ClientSession>();

            _dbSessionTimeoutSec = 10;

            _cleanupTimeoutSec = 600; // ten minutes
            _cleanupTimeoutSec = ServiceInstances.Configuration.GetValue("ClientSessionTimeoutSec", _cleanupTimeoutSec);

            // ensure some constraints
            //
            // minimal session timeout is 1 minute
            if (_cleanupTimeoutSec < 60) _cleanupTimeoutSec = 60;
            // session timeout is no more than 1 hour; if more is required, client should make a session renew request
            if (_cleanupTimeoutSec > 3600) _cleanupTimeoutSec = 3600;

            _staleSessionDropThreadExit = new ManualResetEvent(false);
            new Thread(staleSessionDropThread).Start();
        }

        public void Dispose()
        {
            _staleSessionDropThreadExit.Set();
        }

        public int ClientKeepalivePeriodSec { get { return _cleanupTimeoutSec / 2; } }

        public string LoginUser(IPEndPoint ep, LoginType loginType, 
            User.Role role, int estatedeveloperId, string login, string password)
        {
            // Flood prevention.
            // This is not a DoS prevention, rather a password brute-force stopper.
            ServiceInstances.FloodStopper.UpdatePeer(ep.Address.ToString());

            ServiceInstances.Logger.Info("User login attempt from {0}: type='{1}', login='{2}'.",
                ep, loginType, login);

            User user;
            bool dropOffConcurrentSessions;
            // Anonymous web client login
            if ((LoginType.Plain == loginType) && (User.Role.Visitor == role) && login.Equals("web") && password.Equals("web"))
            {
                user = new User(null, User.Role.Visitor);
                dropOffConcurrentSessions = false;
            }
            else
            {
                user = UserManager.Login(loginType, role, estatedeveloperId, login, password);
                // TODO: Drop off other user sessions? Configurable?
                dropOffConcurrentSessions = true;
            }

            if (user != null)
            {
                ServiceInstances.Logger.Info("User login from {0}, type='{1}', login='{2}' accepted.",
                    ep, loginType, login);

                if (dropOffConcurrentSessions)
                {
                    Dictionary<string, ClientSession> toremove = new Dictionary<string, ClientSession>();
                    lock (_sessionList)
                    {
                        foreach (KeyValuePair<string, ClientSession> kvp in _sessionList)
                        {
                            if ((kvp.Value.AuthLoginType == loginType) && (kvp.Value.AuthLogin.Equals(login)))
                                toremove.Add(kvp.Key, kvp.Value);
                        }
                        foreach (string key in toremove.Keys) _sessionList.Remove(key);
                    }
                    foreach (ClientSession cs in toremove.Values) cs.Dispose();

                    if (toremove.Count > 0)
                        ServiceInstances.Logger.Warn("Removed {0} stale session for this login.", toremove.Count);
                }

                string sessionId = Guid.NewGuid().ToString();

                lock (_sessionList)
                    _sessionList.Add(sessionId, new ClientSession(loginType, login, user));
                                
                return sessionId;
            }
            else
            {
                ServiceInstances.Logger.Warn("User login from {0}, type='{1}', login='{2}' denied.",
                    ep, loginType, login);

                return null;
            }
        }

        public ClientSession this[string sessionId]
        {
            get
            {
                ClientSession result;
                lock (_sessionList)
                    if (sessionId != null)
                    {
                        if (_sessionList.TryGetValue(sessionId, out result))
                        {
                            result.Touch();
                            return result;
                        }
                    }
                return null;
            }
        }

        private void staleSessionDropThread()
        {
            while (!_staleSessionDropThreadExit.WaitOne(60000))
            {
                Dictionary<string, ClientSession> toremove = new Dictionary<string, ClientSession>();
                DateTime now = DateTime.UtcNow;

                lock (_sessionList)
                {
                    foreach (KeyValuePair<string, ClientSession> kvp in _sessionList)
                        if (now.Subtract(kvp.Value.LastUsed).TotalSeconds > _cleanupTimeoutSec) 
                            toremove.Add(kvp.Key, kvp.Value);

                    foreach (string sid in toremove.Keys) _sessionList.Remove(sid);
                }

                foreach (ClientSession cs in toremove.Values)
                {
                    cs.Dispose();
                    ServiceInstances.Logger.Info("Removed stale session for {0}", cs);
                }
            }
        }

        private void oldDbSessionReleaseThread()
        {
            while (!_staleSessionDropThreadExit.WaitOne(1000))
            {
                Dictionary<string, ClientSession> toremove = new Dictionary<string, ClientSession>();
                DateTime now = DateTime.UtcNow;

                lock (_sessionList)
                {
                    foreach (KeyValuePair<string, ClientSession> kvp in _sessionList)
                    {
                        // TODO: Detect kvp.Value.DbSession is stale
                        //if (now.Subtract(kvp.Value.LastUsed).TotalSeconds > _cleanupTimeoutSec)
                        //    toremove.Add(kvp.Key, kvp.Value);
                    }

                    foreach (string sid in toremove.Keys) _sessionList.Remove(sid);
                }

                foreach (ClientSession cs in toremove.Values) cs.Dispose();
            }
        }
    }

    internal class ClientSession : IDisposable
    {
        public LoginType AuthLoginType { get; private set; }
        public string AuthLogin { get; private set; }
        public User User { get; private set; }
        public ISession DbSession { get; private set; }
        private int _dbSessionUseCount;
        public DateTime LastUsed { get; private set; }
        /// <summary>
        /// This session is fully trusted and extended functionality is allowed
        /// </summary>
        public bool TrustedConnection { get; set; }

        public ClientSession(LoginType loginType, string login, User user)
        {
            AuthLoginType = loginType;
            AuthLogin = login;
            User = user;
            TrustedConnection = false;
            DbSession = null;
            _dbSessionUseCount = 0;
            LastUsed = DateTime.UtcNow;
        }

        private ClientSession(LoginType loginType, string login, User user, bool trustedConnection, ISession dbSession)
        {
            AuthLoginType = loginType;
            AuthLogin = login;
            User = user;
            TrustedConnection = trustedConnection;
            DbSession = dbSession;
            _dbSessionUseCount = 0;
            LastUsed = DateTime.UtcNow;
        }

        /// <summary>
        /// Generates a session with virtual non-persistent full-powered user.
        /// </summary>
        public static ClientSession MakeSystemSession()
        {
            return MakeSystemSession(null);
        }

        /// <summary>
        /// Generates a session with virtual non-persistent full-powered user.
        /// </summary>
        public static ClientSession MakeSystemSession(ISession dbSession)
        {
            User u;
            using (UserDao dao = new UserDao(dbSession)) u = dao.GetAnyActiveSuperAdmin();
            if (u != null)
                return new ClientSession(LoginType.Plain, "<system>", u, true, dbSession);
            else
                return new ClientSession(LoginType.Plain, "<system>", new User(null, User.Role.SuperAdmin), true, dbSession);
        }

        /// <summary>
        /// Starts user's database session if it is not started yet.
        /// </summary>
        /// <returns>true if session was started</returns>
        public bool Resume()
        {
            lock (this)
            {
                if (null == DbSession) 
                { 
                    DbSession = NHibernateHelper.GetSession();
                    _dbSessionUseCount = 1;
                    return true; 
                }
                else 
                {
                    if (!DbSession.IsConnected) { DbSession.Reconnect(); _dbSessionUseCount = 1; }
                    else _dbSessionUseCount++;
                    return false; 
                }
            }
        }

        /// <summary>
        /// Shuts down existing user session.
        /// </summary>
        public void Disconnect()
        {
            lock (this)
            {
                if (DbSession != null)
                {
                    if (--_dbSessionUseCount < 1) DbSession.Disconnect();
                }
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                Disconnect();
                if (DbSession != null)
                {
                    //try { DbSession.Flush(); }
                    //catch (Exception e) { e.Equals(this); }
                    DbSession.Close();
                    DbSession.Dispose();
                    DbSession = null;
                }
            }
        }

        public void Touch()
        {
            LastUsed = DateTime.UtcNow;
        }

        public override string ToString()
        {
            switch (User.UserRole)
            {
                case BusinessLogic.User.Role.Buyer:
                    // TODO: embed authentication types
                    return string.Format("buyer-plain-{0}", AuthLogin);

                case BusinessLogic.User.Role.DeveloperAdmin:
                    return string.Format("ed{0}-admin-{1}", User.EstateDeveloperID, AuthLogin);

                case BusinessLogic.User.Role.SalesPerson:
                    return string.Format("ed{0}-sales-{1}", User.EstateDeveloperID, AuthLogin);

                case BusinessLogic.User.Role.Subcontractor:
                    return string.Format("ed{0}-subcont-{1}", User.EstateDeveloperID, AuthLogin);

                case BusinessLogic.User.Role.SuperAdmin:
                    return string.Format("superadmin-{0}", AuthLogin);

                case BusinessLogic.User.Role.SellingAgent:
                    return string.Format("sellingagent-{0}", AuthLogin);

                case BusinessLogic.User.Role.Kiosk:
                    return string.Format("ed{0}-kiosk-{1}", User.EstateDeveloperID, AuthLogin);

                case BusinessLogic.User.Role.Visitor:
                    return string.Format("visitor-{0}", AuthLogin);

                default:
                    return "?";
            }
        }
    }
}