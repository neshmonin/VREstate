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

        //public override Suite GetById(int entityId)
        //{
        //    Suite u = base.GetById(entityId);
        //    if ((u != null) && u.Deleted) u = null;
        //    return u;
        //}

        //public override IList<Suite> GetAll()
        //{
        //    return _session.CreateCriteria<Suite>()
        //        .Add(Restrictions.Eq("Deleted", false))
        //        .List<Suite>();
        //}

        public IList<Suite> GetAll(int buildingId)
        {
            return GetAll(buildingId, false);
        }

        public IList<Suite> GetAll(int buildingId, bool includeDeleted)
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

        //public override void CreateOrUpdate(Suite entity)
        //{
        //    if (entity.Deleted) throw new InvalidOperationException("Cannot modify deleted item.");
        //    entity.MarkUpdated();
        //    base.CreateOrUpdate(entity);
        //}

        //public override void Update(Suite entity)
        //{
        //    if (entity.Deleted) throw new InvalidOperationException("Cannot modify deleted item.");
        //    entity.MarkUpdated();
        //    base.Update(entity);
        //}

        //public override void Delete(Suite entity)
        //{
        //    entity.MarkDeleted();
        //    base.Update(entity);
        //}

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