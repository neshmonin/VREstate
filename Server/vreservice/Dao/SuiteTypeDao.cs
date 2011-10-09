using NHibernate;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    public class SuiteTypeDao : UpdateableBaseDao<SuiteType>
    {
        public SuiteTypeDao(ISession session) : base(session) { }
    }
}
