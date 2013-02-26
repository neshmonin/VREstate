using System;
using System.Collections.Generic;
using NHibernate;
using Vre.Server.BusinessLogic;
using NHibernate.Transform;
using System.Collections;

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

        public IList<ViewOrder> GetActiveInBuildings(ViewOrder.ViewOrderProduct product, int[] buildingIdList)
        {
            return _session.CreateSQLQuery(@"SELECT vo.* FROM ViewOrders vo 
INNER JOIN Suites s ON s.AutoID=vo.TargetObjectId
WHERE vo.Product=:pr AND vo.TargetObjectType=:ty AND vo.Deleted=0 AND vo.[Enabled]=1 AND vo.ExpiresOn>GETUTCDATE()
AND s.BuildingID IN (:bids)").AddEntity(typeof(ViewOrder))
                .SetEnum("pr", product)
                .SetEnum("ty", ViewOrder.SubjectType.Suite)
                .SetParameterList("bids", buildingIdList)
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
        
        public IList<ViewOrder> GetExpiringBeforeNotNotified(DateTime utcTimeLimit, int notificationLevel)
        {
            IQuery q;
            if (notificationLevel >= 0)
            {
                q = _session.CreateQuery("FROM Vre.Server.BusinessLogic.ViewOrder"
                    + " WHERE Deleted=0 AND Enabled=1"
                    + " AND ExpiresOn<:ex AND NotificationsSent=:nl");
            }
            else
            {
                q = _session.CreateQuery("FROM Vre.Server.BusinessLogic.ViewOrder"
                    + " WHERE Deleted=0 AND Enabled=1"
                    + " AND ExpiresOn<:ex AND NotificationsSent<:nl");
                notificationLevel = -notificationLevel;
            }
            //q.SetTime("nex", DateTime.UtcNow);
            q.SetTime("ex", utcTimeLimit);
            q.SetInt32("nl", notificationLevel);
            return q.List<ViewOrder>();
        }

        public IList<ViewOrder> GetAllActive()
        {
            return _session.CreateQuery(
                "FROM Vre.Server.BusinessLogic.ViewOrder "
                + "WHERE Deleted=0 AND Enabled=1").List<ViewOrder>();
        }

        public IList<KeyValuePair<Guid, string>> GetAllActiveIdsAndMlsId()
        {
            return _session.CreateSQLQuery("SELECT vo.[AutoID], vo.[MlsId] FROM [ViewOrders] vo WHERE vo.[Deleted]=0 AND vo.[Enabled]=1")
                .SetResultTransformer(new GuidStringTupleTransformer())
                //.AddEntity(typeof(string))
                .List<KeyValuePair<Guid, string>>();
        }

        class GuidStringTupleTransformer : IResultTransformer
        {
            public IList TransformList(IList collection) { return collection; }

            public object TransformTuple(object[] tuple, string[] aliases)
            {
                return new KeyValuePair<Guid, string>((Guid)tuple[0]/*Guid.Parse(tuple[0] as string)*/, tuple[1] as string);
            }
        }
    }
}