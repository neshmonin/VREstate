using NHibernate;
using NHibernate.Criterion;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    public class SiteDao : UpdateableBaseDao<Site>
    {
        public SiteDao(ISession session) : base(session) { }

        public Site[] GetByDeveloperId(int estateDeveloperId, bool includeDeleted)
        {
            lock (_session)
            {
                ICriteria c = _session.CreateCriteria<Site>().Add(Restrictions.Eq("Developer.AutoID", estateDeveloperId));

                if (!includeDeleted)
                    c = c.Add(Restrictions.Eq("Deleted", false));

                return NHibernateHelper.IListToArray<Site>(c.List<Site>());
            }
        }
    }
}