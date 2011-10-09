using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using NHibernate;
using Vre.Server.BusinessLogic;

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

        public string LoginUser(IPEndPoint ep, LoginType loginType, string login, string password)
        {
            // Flood prevention.
            // This is not a DoS prevention, rather a password brute-force stopper.
            ServiceInstances.FloodStopper.UpdatePeer(ep.Address.ToString());

            ServiceInstances.Logger.Info("User login attempt from {0}: type='{1}', login='{2}'.",
                ep, loginType, login);

            User user = UserManager.Login(loginType, login, password);

            if (user != null)
            {
                ServiceInstances.Logger.Info("User login from {0}, type='{1}', login='{2}' accepted.",
                    ep, loginType, login);

                // TODO: Drop off other user sessions? Configurable?
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

                foreach (ClientSession cs in toremove.Values) cs.Dispose();
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

    public class ClientSession : IDisposable
    {
        public LoginType AuthLoginType { get; private set; }
        public string AuthLogin { get; private set; }
        public User User { get; private set; }
        public ISession DbSession { get; private set; }
        public DateTime LastUsed { get; private set; }

        public ClientSession(LoginType loginType, string login, User user)
        {
            AuthLoginType = loginType;
            AuthLogin = login;
            User = user;
            DbSession = null;
            LastUsed = DateTime.UtcNow;
        }

        /// <summary>
        /// Generates a session with virtual non-persistent full-powered user.
        /// </summary>
        internal static ClientSession MakeSystemSession()
        {
            return new ClientSession(LoginType.Plain, "<system>", new User(null, User.Role.SuperAdmin));
        }

        /// <summary>
        /// Starts user's database session if it is not started yet.
        /// </summary>
        /// <returns>true if session was started</returns>
        public bool Resume()
        {
            if (null == DbSession) { DbSession = NHibernateHelper.GetSession(); return true; }
            else return false;
        }

        /// <summary>
        /// Shuts down existing user session.
        /// </summary>
        public void Disconnect()
        {
            if (DbSession != null)
            {
                DbSession.Flush();
                DbSession.Close();
                DbSession.Dispose();
                DbSession = null;
            }
        }

        public void Dispose()
        {
            Disconnect();
        }

        public void Touch()
        {
            LastUsed = DateTime.UtcNow;
        }
    }
}