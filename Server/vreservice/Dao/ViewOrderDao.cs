using System;
using NHibernate;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    internal class ViewOrderDao : GenericDisposableDao<ViewOrder, Guid>
    {
        public ViewOrderDao(ISession session) : base(session) { }

        public ViewOrder Get(int ownerId, ViewOrder.SubjectType type, int targetObjectId)
        {
            return _session.CreateQuery(
                "FROM Vre.Server.BusinessLogic.ViewOrder WHERE OwnerId=:oid "
                + "AND TargetObjectType=:ty AND TargetObjectId=:tid AND Deleted=0")
                .SetInt32("oid", ownerId)
                .SetEnum("ty", type)
                .SetInt32("tid", targetObjectId)
                .UniqueResult<ViewOrder>();
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
    }
}