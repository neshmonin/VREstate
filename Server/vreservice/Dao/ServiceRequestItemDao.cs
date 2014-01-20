using System;
using System.Collections.Generic;
using NHibernate;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    internal class ServiceRequestItemDao : GenericDisposableDao<ServiceRequestItem, int>
    {
        public ServiceRequestItemDao(ISession session) : base(session) { }

		public IEnumerable<ServiceRequestItem> GetByTargetAndTime(
            Invoice.SubjectType targetType, int targetId,
            DateTime fromInclusive, DateTime toExclusive)
		{
			return _session.CreateQuery(
                "FROM Vre.Server.BusinessLogic.ServiceRequestItem WHERE TargetObjectType=:t AND TargetObjectId=:id AND Time>=:from AND Time<:to")
				.SetEnum("t", targetType)
                .SetInt32("id", targetId)
                .SetDateTime("from", fromInclusive)
                .SetDateTime("to", toExclusive)
				.Enumerable<ServiceRequestItem>();
		}
    }
}