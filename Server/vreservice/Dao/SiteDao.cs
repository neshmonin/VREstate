using NHibernate;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    public class SiteDao : UpdateableBaseDao<Site>
    {
        public SiteDao(ISession session) : base(session) { }
    }
}