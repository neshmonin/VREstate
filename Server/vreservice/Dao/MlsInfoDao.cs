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

		public IList<string> GetStubNumbers()
		{
			return _session.CreateSQLQuery("SELECT mi.[MlsNum] FROM [MlsInfo] mi WHERE mi.[Deleted]=0 AND mi.[RawInfo]='{}'")
				.List<string>();
		}
    }
}