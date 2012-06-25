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

        public Site GetById(string id)
        {
            lock (_session)
            {
                int key;
                if (int.TryParse(id, out key)) return GetById(key);

                return _session.CreateCriteria<Site>()
                    .Add(Restrictions.Eq("Name", id))
                    .Add(Restrictions.Eq("Deleted", false))
                    .UniqueResult<Site>();
            }        
        }

        public Site GetById(EstateDeveloper ed, string id)
        {
            lock (_session)
            {
                int key;
                if (int.TryParse(id, out key)) return GetById(key);

                foreach (Site s in ed.Sites)
                    if (s.Name.Equals(id) && !s.Deleted) return s;
            }

            return null;
        }
    }
}