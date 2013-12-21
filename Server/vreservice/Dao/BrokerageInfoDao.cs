using System.Collections.Generic;
using NHibernate;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    internal class BrokerageInfoDao : UpdateableBaseDao<BrokerageInfo>
    {
		public BrokerageInfoDao(ISession session) : base(session) { }
		/*
		public IList<BrokerageInfo> Get()
		{
			return _session.CreateQuery(
				"FROM Vre.Server.BusinessLogic.BrokerageInfo WHERE Deleted=0")
				.List<BrokerageInfo>();
		}

		public IList<BrokerageInfo> Get(bool includeDeleted)
		{
			if (!includeDeleted)
				return Get();
			else
				return _session.CreateQuery(
					"FROM Vre.Server.BusinessLogic.BrokerageInfo")
					.List<BrokerageInfo>();
		}
		 */
	}
}