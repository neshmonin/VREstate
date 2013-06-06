using System.Collections.Generic;
using NHibernate;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    internal class NamedSearchFilterDao : GenericDisposableDao<NamedSearchFilter, int>
    {
		public NamedSearchFilterDao(ISession session) : base(session) { }

		public IList<NamedSearchFilter> Get()
		{
			return _session.CreateQuery(
				"FROM Vre.Server.BusinessLogic.NamedSearchFilter WHERE Deleted=0")
				.List<NamedSearchFilter>();
		}

		public IList<NamedSearchFilter> Get(int ownerId)
        {
			return _session.CreateQuery(
				"FROM Vre.Server.BusinessLogic.NamedSearchFilter WHERE OwnerId=:oid AND Deleted=0")
                .SetInt32("oid", ownerId)
				.List<NamedSearchFilter>();
        }

		public IList<NamedSearchFilter> Get(int ownerId, bool includeDeleted)
        {
            if (includeDeleted)
				return _session.CreateQuery(
					"FROM Vre.Server.BusinessLogic.NamedSearchFilter WHERE OwnerId=:oid")
                    .SetInt32("oid", ownerId)
					.List<NamedSearchFilter>();
            else
                return Get(ownerId);
        }
    }
}