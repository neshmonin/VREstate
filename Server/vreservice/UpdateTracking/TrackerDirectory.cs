using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Vre.Server.BusinessLogic;
using Vre.Server.RemoteService;

namespace Vre.Server.UpdateTracking
{
    internal class TrackerDirectory
    {
        // Have to use second-level dictionaries as concurrent collections do not provide
        // a ISet<T> functionality; value is an unused dummy.
        private ConcurrentDictionary<int, ConcurrentDictionary<ClientSession, int>> _trackedBuildings;
        private ConcurrentDictionary<int, ConcurrentDictionary<ClientSession, int>> _trackedSuites;

        public TrackerDirectory()
        {
            _trackedBuildings = new ConcurrentDictionary<int, ConcurrentDictionary<ClientSession, int>>();
            _trackedSuites = new ConcurrentDictionary<int, ConcurrentDictionary<ClientSession, int>>();
        }

        #region generic mainenance
        private static void addSubscriptions(
            ConcurrentDictionary<int, ConcurrentDictionary<ClientSession, int>> directory,
            ClientSession session, IEnumerable<int> idList)
        {
            ConcurrentDictionary<ClientSession, int> newSet = new ConcurrentDictionary<ClientSession, int>();
            foreach (int id in idList)
            {
                ConcurrentDictionary<ClientSession, int> sessions = directory.GetOrAdd(id, newSet);
                sessions.AddOrUpdate(session, 0, (s, o) => 0);
                if (ReferenceEquals(sessions, newSet)) newSet = new ConcurrentDictionary<ClientSession, int>();  // prepare another copy if we used one right now
            }
        }

        private static void removeSubscriptions(
            ConcurrentDictionary<int, ConcurrentDictionary<ClientSession, int>> directory,
            ClientSession session, IEnumerable<int> idList)
        {
            ConcurrentDictionary<ClientSession, int> newSet = new ConcurrentDictionary<ClientSession, int>();
            foreach (int id in idList)
            {
                ConcurrentDictionary<ClientSession, int> sessions = directory.GetOrAdd(id, newSet);
                int dummy;
                sessions.TryRemove(session, out dummy);
                if (ReferenceEquals(sessions, newSet)) newSet = new ConcurrentDictionary<ClientSession, int>();  // prepare another copy if we used one right now
            }
        }
        #endregion

        public void SubscribeToBuildings(ClientSession session, IEnumerable<int> buildingIds)
        {
            addSubscriptions(_trackedBuildings, session, buildingIds);
        }

        public void SubscribeToSuites(ClientSession session, IEnumerable<int> suiteIds)
        {
            addSubscriptions(_trackedSuites, session, suiteIds);
        }

        public void UnsubscribeFromBuildings(ClientSession session, IEnumerable<int> buildingIds)
        {
            removeSubscriptions(_trackedBuildings, session, buildingIds);
        }

        public void UnsubscribeFromSuites(ClientSession session, IEnumerable<int> suiteIds)
        {
            removeSubscriptions(_trackedSuites, session, suiteIds);
        }

        //public void NotifyModified(UpdateableBase o)
        //{
        //    Type t = o.GetType();
        //    if (t.Equals(typeof(Suite))) NotifyModifiedSuite(o.AutoID);
        //    else if (t.Equals(typeof(Building))) NotifyModifiedBuilding(o.AutoID);
        //}

        //public void NotifyModifiedBuilding(int id)
        //{
        //    foreach (ClientSession session in _trackedBuildings.GetOrAdd(id, new ConcurrentDictionary<ClientSession, int>()).Keys)
        //        session.NotifyModifiedBuilding(id);
        //}

        //public void NotifyModifiedSuite(int id)
        //{
        //    foreach (ClientSession session in _trackedSuites.GetOrAdd(id, new ConcurrentDictionary<ClientSession, int>()).Keys)
        //        session.NotifyModifiedSuite(id);
        //}

        public void NotifyModified(IEnumerable<UpdateableBase> ol)
        {
            // sort updateable items into building and suite ids
            //
            int mcnt = ol.Count();
            List<int> modifiedBuildings = new List<int>(mcnt);
            List<int> modifiedSuites = new List<int>(mcnt);

            foreach (UpdateableBase o in ol)
            {
                Type t = o.GetType();
                if (t.Equals(typeof(Suite))) modifiedSuites.Add(o.AutoID);
                else if (t.Equals(typeof(Building))) modifiedBuildings.Add(o.AutoID);
            }
            if ((modifiedBuildings.Count < 1) && (modifiedSuites.Count < 1))  return;

            // retrieve all relevant recipient sessions
            //
            HashSet<ClientSession> recipients = new HashSet<ClientSession>();

            foreach (int id in modifiedBuildings)
            {
                ConcurrentDictionary<ClientSession, int> sessionList = null;
                if (_trackedBuildings.TryGetValue(id, out sessionList))
                {
                    foreach (ClientSession cs in sessionList.Keys) recipients.Add(cs);
                }
            }
            foreach (int id in modifiedSuites)
            {
                ConcurrentDictionary<ClientSession, int> sessionList = null;
                if (_trackedSuites.TryGetValue(id, out sessionList))
                {
                    foreach (ClientSession cs in sessionList.Keys) recipients.Add(cs);
                }
            }

            // Call recipients in a loop
            // TODO: PERFORMANCE: there are locks in the called method which may block parallel run of this method
            foreach (ClientSession cs in recipients) cs.NotifyModified(modifiedBuildings, modifiedSuites);
        }
    }
}