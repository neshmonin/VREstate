using System;
using System.Collections.Generic;
using NHibernate;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    internal class ViewOrderDao : GenericDisposableDao<ViewOrder, Guid>
    {
        public ViewOrderDao(ISession session) : base(session) { }

        public ViewOrder GetActive(int ownerId, ViewOrder.SubjectType type, int targetObjectId)
        {
            return _session.CreateQuery(
                "FROM Vre.Server.BusinessLogic.ViewOrder WHERE OwnerId=:oid "
                + "AND TargetObjectType=:ty AND TargetObjectId=:tid AND Deleted=0 AND Enabled=1 AND ExpiresOn>:ex")
                .SetInt32("oid", ownerId)
                .SetEnum("ty", type)
                .SetInt32("tid", targetObjectId)
                .SetDateTime("ex", DateTime.UtcNow)
                .UniqueResult<ViewOrder>();
        }

        public IList<ViewOrder> GetActive(ViewOrder.ViewOrderProduct product, ViewOrder.SubjectType type, int targetObjectId)
        {
            return _session.CreateQuery(
                "FROM Vre.Server.BusinessLogic.ViewOrder WHERE Product=:pr "
                + "AND TargetObjectType=:ty AND TargetObjectId=:tid AND Deleted=0 AND Enabled=1 AND ExpiresOn>:ex")
                .SetEnum("pr", product)
                .SetEnum("ty", type)
                .SetInt32("tid", targetObjectId)
                .SetDateTime("ex", DateTime.UtcNow)
                .List<ViewOrder>();
        }

        public IList<ViewOrder> GetActive(ViewOrder.SubjectType type, int targetObjectId)
        {
            return _session.CreateQuery(
                "FROM Vre.Server.BusinessLogic.ViewOrder "
                + "WHERE TargetObjectType=:ty AND TargetObjectId=:tid AND Deleted=0 AND Enabled=1 AND ExpiresOn>:ex")
                .SetEnum("ty", type)
                .SetInt32("tid", targetObjectId)
                .SetDateTime("ex", DateTime.UtcNow)
                .List<ViewOrder>();
        }

        public IList<ViewOrder> GetActiveSameBuilding(ViewOrder.ViewOrderProduct product, Suite suite)
        {
            return _session.CreateSQLQuery(@"SELECT vo.* FROM ViewOrders vo 
INNER JOIN Suites s ON s.AutoID=vo.TargetObjectId
WHERE vo.Product=:pr AND vo.TargetObjectType=:ty AND vo.Deleted=0 AND vo.[Enabled]=1 AND vo.ExpiresOn>GETUTCDATE()
AND s.BuildingID=:bid").AddEntity(typeof(ViewOrder))
                .SetEnum("pr", product)
                .SetEnum("ty", ViewOrder.SubjectType.Suite)
                .SetInt32("bid", suite.Building.AutoID)
                .List<ViewOrder>();
        }

        public ViewOrder[] Get(int ownerId)
        {
            return NHibernateHelper.IListToArray<ViewOrder>(_session.CreateQuery(
                "FROM Vre.Server.BusinessLogic.ViewOrder WHERE OwnerId=:oid AND Deleted=0")
                .SetInt32("oid", ownerId)
                .List<ViewOrder>());
        }

        public ViewOrder[] Get(int ownerId, bool includeDeleted)
        {
            if (includeDeleted)
                return NHibernateHelper.IListToArray<ViewOrder>(_session.CreateQuery(
                    "FROM Vre.Server.BusinessLogic.ViewOrder WHERE OwnerId=:oid")
                    .SetInt32("oid", ownerId)
                    .List<ViewOrder>());
            else
                return Get(ownerId);
        }

        public IList<ViewOrder> GetAllExpiredStillActive(ViewOrder.SubjectType targetObjectType)
        {
            IQuery q = _session.CreateQuery("FROM Vre.Server.BusinessLogic.ViewOrder"
                + " WHERE TargetObjectType=:tot"
                + " AND Deleted=0 AND Enabled=1 AND ExpiresOn<:ex");
            q.SetParameter<ViewOrder.SubjectType>("tot", targetObjectType);
            q.SetTime("ex", DateTime.UtcNow);
            return q.List<ViewOrder>();
        }

        //public IList<ViewOrder> GetActive(User owner)
        //{
        //    IQuery q = _session.CreateQuery("FROM Vre.Server.BusinessLogic.ViewOrder"
        //        + " WHERE OwnerId=:oid AND Deleted=0");
        //    q.SetInt32("oid", owner.AutoID);
        //    return q.List<ViewOrder>();
        //}
    }
}