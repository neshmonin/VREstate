using System;
using System.Collections.Generic;
using System.Text;
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

        public Building GetById(string id)
        {
            lock (_session)
            {
                int key;
                if (int.TryParse(id, out key)) return GetById(key);

                return _session.CreateCriteria<Building>()
                    .Add(Restrictions.Eq("Name", id))
                    .Add(Restrictions.Eq("Deleted", false))
                    .UniqueResult<Building>();
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

        public IList<Building> SearchByAddress(string country, string postalCode, string state, string municipality,
            string addressFreeText)
        {
            lock (_session)
            {
                StringBuilder qs = new StringBuilder("FROM Vre.Server.BusinessLogic.Building WHERE");
                int filterCriteriaCnt = 0;

                if (country != null) 
                { 
                    qs.Append(" Country=:co"); filterCriteriaCnt++; 
                }
                if (postalCode != null) 
                { 
                    if (filterCriteriaCnt > 0) qs.Append(" AND"); 
                    qs.Append(" PostalCode=:po"); filterCriteriaCnt++; 
                }
                if (state != null) 
                { 
                    if (filterCriteriaCnt > 0) qs.Append(" AND"); 
                    qs.Append(" StateProvince=:stpr"); filterCriteriaCnt++; 
                }
                if (municipality != null) 
                { 
                    if (filterCriteriaCnt > 0) qs.Append(" AND"); 
                    qs.Append(" City=:mu"); filterCriteriaCnt++; 
                }
                if (addressFreeText != null) 
                { 
                    if (filterCriteriaCnt > 0) qs.Append(" AND"); 
                    qs.Append(" AddressLine1+' '+AddressLine2 LIKE :fa"); filterCriteriaCnt++; 
                }

                if (0 == filterCriteriaCnt) throw new ArgumentException("Need at least one address criterion");

                IQuery q = _session.CreateQuery(qs.ToString());

                if (country != null) q = q.SetString("co", country);
                if (postalCode != null) q = q.SetString("po", postalCode);
                if (state != null) q = q.SetString("stpr", state);
                if (municipality != null) q = q.SetString("mu", municipality);
                if (addressFreeText != null) q = q.SetString("fa", addressFreeText.Replace('*', '%'));
                    
                return q.List<Building>();

                //return _session.CreateQuery(
                //    "FROM Vre.Server.BusinessLogic.Building WHERE Country=:co " 
                //    + "AND PostalCode=:po AND StateProvince=:stpr AND City=:mu "
                //    + "AND AddressLine1+' '+AddressLine2 LIKE :fa")
                //    .SetString("co", country)
                //    .SetString("po", postalCode)
                //    .SetString("stpr", state)
                //    .SetString("mu", municipality)
                //    .SetString("fa", addressFreeText.Replace('*', '%'))
                //    .List<Building>();
            }
        }
    }
}