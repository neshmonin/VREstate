using System.Collections.Generic;
using NHibernate;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    internal class MlsInfoDao : GenericDisposableDao<MlsInfo, int>
    {
		public MlsInfoDao(ISession session) : base(session) { }

		public MlsInfo GetByMlsNum(string num)
		{
			return _session.CreateQuery(
				"FROM Vre.Server.BusinessLogic.MlsInfo WHERE MlsNum=:num")
				.SetString("num", num)
				.UniqueResult<MlsInfo>();
		}
    }
}