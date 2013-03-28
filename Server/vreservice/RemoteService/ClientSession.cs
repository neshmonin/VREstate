using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;

namespace Vre.Server.RemoteService
{
    internal class ClientSessionStore : IDisposable
    {
        private ConcurrentDictionary<string, ClientSession> _sessionList;
        private ManualResetEvent _staleSessionDropThreadExit;
        private int _cleanupTimeoutSec;

        public ClientSessionStore()
        {
            _sessionList = new ConcurrentDictionary<string, ClientSession>();

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
                    int cnt = 0;
                    foreach (KeyValuePair<string, ClientSession> kvp in _sessionList)
                    {
                        ClientSession cs;
                        if ((kvp.Value.AuthLoginType == loginType) && (kvp.Value.AuthLogin.Equals(login)))
                            if (_sessionList.TryRemove(kvp.Key, out cs))
                            {
                                cs.Dispose();
                                cnt++;
                            }
                    }

                    if (cnt > 0)
                        ServiceInstances.Logger.Warn("Removed {0} stale session for this login.", cnt);
                }

                string sessionId = Guid.NewGuid().ToString();

                // shall never fail as we've just made up a random unique key
                _sessionList.TryAdd(sessionId, new ClientSession(loginType, login, user));
                                
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
                if (_sessionList.TryGetValue(sessionId, out result))
                {
                    result.Touch();
                    return result;
                }
                return null;
            }
        }

        public void DropSession(string sessionId)
        {
            ClientSession cs;
            if (_sessionList.TryRemove(sessionId, out cs))
                cs.Dispose();
        }

        private void staleSessionDropThread()
        {
            Thread.CurrentThread.Name = "StaleSessionDropper#" + Thread.CurrentThread.ManagedThreadId.ToString();
            while (!_staleSessionDropThreadExit.WaitOne(60000))
            {
                ClientSession cs;
                DateTime now = DateTime.UtcNow;

                foreach (KeyValuePair<string, ClientSession> kvp in _sessionList)
                    if (now.Subtract(kvp.Value.LastUsed).TotalSeconds > _cleanupTimeoutSec) 
                        if (_sessionList.TryRemove(kvp.Key, out cs))
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
                DateTime now = DateTime.UtcNow;

                foreach (KeyValuePair<string, ClientSession> kvp in _sessionList)
                    // TODO: Detect kvp.Value.DbSession is stale
                    if (now.Subtract(kvp.Value.LastUsed).TotalSeconds > _cleanupTimeoutSec)
                    {
                        //kvp.Value.Disconnect(true);
                    }
            }
        }
    }

    internal class ClientSession : IDisposable
    {
        private readonly object _dbSessionLock = new object();
        private readonly object _subscriptionMgmtLock = new object();
        private readonly object _eventThreadLock = new object();

        public LoginType AuthLoginType { get; private set; }
        public string AuthLogin { get; private set; }
        public User User { get; private set; }
        public ISession DbSession { get; private set; }
        private readonly Mutex _dbSessionMutex = new Mutex();
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
            _dbSessionMutex.WaitOne();
            lock (_dbSessionLock)
            {
                if (0 == _dbSessionUseCount++)
                {
                    if (null == DbSession) DbSession = NHibernateHelper.GetSession();
                    else if (!DbSession.IsConnected) DbSession.Reconnect();
                }
            }
            return false;
        }

        /// <summary>
        /// Shuts down existing user session.
        /// </summary>
        public void Disconnect(bool emergency)
        {
            lock (_dbSessionLock)
            {
                if (emergency)
                {
                    if (DbSession != null)
                    {
                        DbSession.Dispose();
                        DbSession = null;
                        _dbSessionUseCount = 0;
                    }
                }
                else if (_dbSessionUseCount > 0)
                {
                    if ((_dbSessionUseCount-- < 1) && (DbSession != null))
                        DbSession.Disconnect();
                }
            }
            _dbSessionMutex.ReleaseMutex();
        }

        public void Dispose()
        {
            killEventThread();
            UnsubscribeAll();
            lock (_dbSessionLock)
            {
                if (DbSession != null)
                {
                    DbSession.Dispose();
                    DbSession = null;
                    _dbSessionUseCount = 0;
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

        private readonly HashSet<int> _subscribedBuildings = new HashSet<int>();
        private readonly HashSet<int> _subscribedSuites = new HashSet<int>();
        private readonly HashSet<int> _modifiedBuildings = new HashSet<int>();
        private readonly HashSet<int> _modifiedSuites = new HashSet<int>();
        private readonly AutoResetEvent _subscribedItemModified = new AutoResetEvent(false);

        public void Subscribe(IEnumerable<Building> buildings)
        {
            ICollection<int> toRemove, toAdd;
            ICollection<int> newItems = new List<int>(buildings.ConvertTo(b => b.AutoID));

            lock (_subscriptionMgmtLock)
            {
                CollectionExtensions.Intersect<int>(_subscribedBuildings, newItems, out toRemove, out toAdd);

                ServiceInstances.EntityUpdateTracker.UnsubscribeFromBuildings(this, toRemove);
                ServiceInstances.EntityUpdateTracker.SubscribeToBuildings(this, toAdd);

                foreach (int id in toRemove) _modifiedBuildings.Remove(id);
                _subscribedBuildings.Clear();
                foreach (int id in newItems) _subscribedBuildings.Add(id);
            }
        }

        public void Subscribe(IEnumerable<Suite> suites)
        {
            ICollection<int> toRemove, toAdd;
            ICollection<int> newItems = new List<int>(suites.ConvertTo(b => b.AutoID));

            lock (_subscriptionMgmtLock)
            {
                CollectionExtensions.Intersect<int>(_subscribedSuites, newItems, out toRemove, out toAdd);

                ServiceInstances.EntityUpdateTracker.UnsubscribeFromSuites(this, toRemove);
                ServiceInstances.EntityUpdateTracker.SubscribeToSuites(this, toAdd);

                foreach (int id in toRemove) _modifiedSuites.Remove(id);
                _subscribedSuites.Clear();
                foreach (int id in newItems) _subscribedSuites.Add(id);
            }
        }

        public void UnsubscribeAll()
        {
            lock (_subscriptionMgmtLock)
            {
                if (_subscribedBuildings.Count > 0)
                {
                    ServiceInstances.EntityUpdateTracker.UnsubscribeFromBuildings(this, _subscribedBuildings);
                    _subscribedBuildings.Clear();
                }
                if (_subscribedSuites.Count > 0)
                {
                    ServiceInstances.EntityUpdateTracker.UnsubscribeFromSuites(this, _subscribedSuites);
                    _subscribedSuites.Clear();
                }
                _modifiedBuildings.Clear();
                _modifiedSuites.Clear();
            }
        }

        //public void NotifyModifiedBuilding(int id)
        //{
        //    bool changed = false;
        //    lock (_subscriptionMgmtLock)
        //    {
        //        if (!_modifiedBuildings.Contains(id))
        //        {
        //            _modifiedBuildings.Add(id);
        //            changed = true;
        //        }
        //    }
        //    if (changed) _subscribedItemModified.Set();
        //}

        //public void NotifyModifiedSuite(int id)
        //{
        //    bool changed = false;
        //    lock (_subscriptionMgmtLock)
        //    {
        //        if (!_modifiedSuites.Contains(id))
        //        {
        //            _modifiedSuites.Add(id);
        //            changed = true;
        //        }
        //    }
        //    if (changed) _subscribedItemModified.Set();
        //}

        public void NotifyModified(IEnumerable<int> buildingIds, IEnumerable<int> suiteIds)
        {
            bool changed = false;
            lock (_subscriptionMgmtLock)
            {
                foreach (int id in buildingIds)
                    if (_subscribedBuildings.Contains(id) && !_modifiedBuildings.Contains(id))
                    {
                        _modifiedBuildings.Add(id);
                        changed = true;
                    }

                foreach (int id in suiteIds)
                    if (_subscribedSuites.Contains(id) && !_modifiedSuites.Contains(id))
                    {
                        _modifiedSuites.Add(id);
                        changed = true;
                    }
            }
            if (changed) _subscribedItemModified.Set();
        }

        private void buildModifiedResponse(out IList<Building> buildings, out IList<Suite> suites)
        {
            try
            {
                int[] buildingIds, suiteIds;

                lock (_subscriptionMgmtLock)
                {
                    buildingIds = _modifiedBuildings.ToArray();
                    suiteIds = _modifiedSuites.ToArray();
                    _modifiedBuildings.Clear();
                    _modifiedSuites.Clear();
                }

                Building[] bresult = new Building[buildingIds.Length];
                Suite[] sresult = new Suite[suiteIds.Length];

                using (BuildingDao dao = new BuildingDao(DbSession))
                {
                    for (int idx = buildingIds.Length - 1; idx >= 0; idx--)
                    {
                        DbSession.Evict(DbSession.Load<Building>(buildingIds[idx]));  // for sure we do have it cached!
                        bresult[idx] = dao.GetById(buildingIds[idx]);
                    }
                }
                using (SuiteDao dao = new SuiteDao(DbSession))
                {
                    for (int idx = suiteIds.Length - 1; idx >= 0; idx--)
                    {
                        DbSession.Evict(DbSession.Load<Suite>(suiteIds[idx]));  // for sure we do have it cached!
                        sresult[idx] = dao.GetById(suiteIds[idx]);
                    }
                }

                buildings = bresult;
                suites = sresult;
            }
            catch
            {
                buildings = new Building[0];
                suites = new Suite[0];
            }
        }

        public IEnumerable<int> GetModifiedBuildingIds()
        {
            lock (_subscriptionMgmtLock) return _modifiedBuildings.ToArray();
        }

        public IEnumerable<int> GetModifiedSuiteIds()
        {
            lock (_subscriptionMgmtLock) return _modifiedSuites.ToArray();
        }

        private Thread _eventThread;
        private string _eventThreadName = null;
        private readonly ManualResetEvent _exitEventThread = new ManualResetEvent(false);
        private int _eventThreadCounter = 0;

        public void StartEventThread(IResponseData response)
        {
            int status = Interlocked.Increment(ref _eventThreadCounter);
            if (status > 1)
            {
                _exitEventThread.Set();
                if (!_eventThread.Join(100))
                    ServiceInstances.Logger.Error("Event thread {0} timed out quitting and is orphaned.", _eventThreadName ?? "<unknown>");
            }
            lock (_eventThreadLock)
            {
                _exitEventThread.Reset();
                _eventThread = new Thread(eventThread);
                _eventThread.IsBackground = true;
                _eventThread.Start(response);
            }
        }

        private void killEventThread()
        {
            if (_eventThreadCounter > 0)
            {
                _exitEventThread.Set();
                if (!_eventThread.Join(100))
                    ServiceInstances.Logger.Error("Event thread {0} timed out quitting and is orphaned.", _eventThreadName ?? "<unknown>");
            }
        }

        private void eventThread(object param)
        {
            IResponseData ctx = param as IResponseData;
            bool dbSessionResumed = false;
            if (null == ctx) return;

            try
            {
                Thread.CurrentThread.Name = "HttpPush#" + Thread.CurrentThread.ManagedThreadId.ToString();
                _eventThreadName = Thread.CurrentThread.Name;

                IList<Building> buildings = null;
                IList<Suite> suites = null;

                WaitHandle[] events = new WaitHandle[] { _subscribedItemModified, _exitEventThread };
                switch (WaitHandle.WaitAny(events, ServiceInstances.SessionStore.ClientKeepalivePeriodSec * 1000))
                {
                    case WaitHandle.WaitTimeout:  // provide an empty response
                        dbSessionResumed = true;
                        Resume();  // TODO: Response builder currently NEEDS session; find a way to avoid this.
                        buildings = new Building[0];
                        suites = new Suite[0];
                        break;

                    case 0:  // provide a response
                        buildings = new List<Building>();
                        suites = new List<Suite>();
                        dbSessionResumed = true;
                        Resume();
                        buildModifiedResponse(out buildings, out suites);
                        break;

                    case 1:  // abort connection immediately
                        break;
                }

                if ((buildings != null) && (suites != null))
                    DataService.GenerateEventDataResponse(this.DbSession, ref ctx, ref buildings, ref suites);

                if (dbSessionResumed) { Disconnect(false); dbSessionResumed = false; }

                // close
                ctx.ProcessResponse();
            }
            catch (NHibernate.HibernateException ex)
            {
                Disconnect(true);  // no need for check; if we get this exception - session WAS resumed.
                ctx.ProcessResponse(ex);
            }
            catch (Exception ex)
            {
                if (dbSessionResumed) Disconnect(false);
                ctx.ProcessResponse(ex);
            }
            finally
            {
                Interlocked.Decrement(ref _eventThreadCounter);
            }
        }     
    }
}