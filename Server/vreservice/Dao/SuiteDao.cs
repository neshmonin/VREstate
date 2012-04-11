using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    public class SuiteDao : UpdateableBaseDao<Suite>
    {
        public SuiteDao(ISession session) : base(session) { }

        public IList<Suite> GetAll(int buildingId)
        {
            lock (_session) return GetAll(buildingId, false);
        }

        public IList<Suite> GetAll(int buildingId, bool includeDeleted)
        {
            lock (_session)
            {
                if (includeDeleted)
                    return _session.CreateCriteria<Suite>()
                        .Add(Restrictions.Eq("BuildingID", buildingId))
                        .List<Suite>();
                else
                    return _session.CreateCriteria<Suite>()
                        .Add(Restrictions.Eq("BuildingID", buildingId) && Restrictions.Eq("Deleted", false))
                        .List<Suite>();
            }
        }

        //public int DeleteSuites(Building building)
        //{
        //    int result;
        //    lock (_session)
        //    {
        //        IQuery q = _session.CreateQuery("UPDATE Vre.Server.BusinessLogic.Suite u SET u.Updated = GETUTCDATE(), u.Deleted = 1 WHERE u.Building = :id");
        //        q = q.SetEntity("id", building);
        //        result = q.ExecuteUpdate();

        //        if (_forcedFlush) _session.Flush();

        //        _session.Refresh(building);  // make sure building object got them unloaded
        //    }
        //    return result;
        //}
    }
}