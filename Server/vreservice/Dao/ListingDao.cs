using System;
using Vre.Server.BusinessLogic;
using NHibernate;
using NHibernate.Criterion;

namespace Vre.Server.Dao
{
    internal class ListingDao : GenericDisposableDao<Listing, Guid>
    {
        public ListingDao(ISession session) : base(session) { }

        public Listing Get(int ownerId, Listing.SubjectType type, int targetObjectId)
        {
            return _session.CreateQuery(
                "FROM Vre.Server.BusinessLogic.Listing WHERE OwnerId=:oid "
                + "AND TargetObjectType=:ty AND TargetObjectId=:tid AND Deleted=0")
                .SetInt32("oid", ownerId)
                .SetEnum("ty", type)
                .SetInt32("tid", targetObjectId)
                .UniqueResult<Listing>();
        }
    }
}