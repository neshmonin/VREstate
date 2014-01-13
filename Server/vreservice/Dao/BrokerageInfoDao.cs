using System.Collections.Generic;
using NHibernate;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    internal class BrokerageInfoDao : UpdateableBaseDao<BrokerageInfo>
    {
		public BrokerageInfoDao(ISession session) : base(session) { }

		public BrokerageInfo GetByName(string name)
		{
			return _session.CreateQuery(
				"FROM Vre.Server.BusinessLogic.BrokerageInfo WHERE Deleted=0 AND Name=:name")
				.SetString("name", name)
				.UniqueResult<BrokerageInfo>();
		}

		public bool Exists(string name)
		{
			return GetByName(name) != null;
		}
	}
}