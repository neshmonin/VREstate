using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    public class SuiteTypeDao : UpdateableBaseDao<SuiteType>
    {
        public SuiteTypeDao(ISession session) : base(session) { }

        public SuiteType GetBySiteAndName(int siteId, string name)
        {
            return _session.CreateCriteria<SuiteType>()
                .Add(Restrictions.Eq("SiteID", siteId))
                .Add(Restrictions.Eq("Name", name))
                .UniqueResult<SuiteType>();
        }

        public IList<SuiteType> GetBySite(int siteId)
        {
            return _session.CreateCriteria<SuiteType>()
                .Add(Restrictions.Eq("SiteID", siteId))
                .List<SuiteType>();
        }
    }
}
