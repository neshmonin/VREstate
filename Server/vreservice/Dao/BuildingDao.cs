using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    public class BuildingDao : UpdateableBaseDao<Building>
    {
        public BuildingDao(ISession session) : base(session) { }

        public IList<Building> GetAll(int siteId)
        {
            return GetAll(siteId, false);
        }

        public IList<Building> GetAll(int siteId, bool includeDeleted)
        {
            lock (_session)
            {
                if (includeDeleted)
                    return _session.CreateCriteria<Building>()
                        .Add(Restrictions.Eq("ConstructionSite.AutoID", siteId))
                        .List<Building>();
                else
                    return _session.CreateCriteria<Building>()
                        .Add(Restrictions.Eq("ConstructionSite.AutoID", siteId) && Restrictions.Eq("Deleted", false))
                        .List<Building>();
            }
        }

        //public int DeleteBuildings(int estateDeveloperId)
        //{
        //    int result = 0;
        //    lock (_session)
        //    {
        //        using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session))
        //        {
        //            using (SuiteDao sdao = new SuiteDao(_session))
        //            {
        //                foreach (Building b in GetAll(estateDeveloperId, false))
        //                {
        //                    sdao.DeleteSuites(b);
        //                    Delete(b);
        //                    result++;
        //                }
        //            }
        //            tran.Commit();
        //        }
        //    }
        //    return result;
        //}
    }
}