using NHibernate;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    public class SuiteLevelDao : GenericDisposableDao<SuiteLevel, int>
    {
        public SuiteLevelDao(ISession session) : base(session) { }
    }
}
