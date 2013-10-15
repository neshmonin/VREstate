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
                StringBuilder qs = new StringBuilder("FROM Vre.Server.BusinessLogic.Building WHERE Deleted=false");
                int filterCriteriaCnt = 0;

                if (country != null) 
                { 
                    qs.Append(" AND (Country=:co OR Country IS NULL)"); filterCriteriaCnt++; 
                }
                if (postalCode != null) 
                { 
                    qs.Append(" AND (PostalCode=:po OR PostalCode IS NULL)"); filterCriteriaCnt++; 
                }
                if (state != null) 
                {
					qs.Append(" AND (StateProvince=:stpr OR StateProvince IS NULL)"); filterCriteriaCnt++; 
                }
                if (municipality != null) 
                { 
                    qs.Append(" AND (City=:mu OR City IS NULL)"); filterCriteriaCnt++; 
                }
                if (addressFreeText != null) 
                { 
                    qs.Append(" AND AddressLine1 LIKE :fa"); filterCriteriaCnt++; 
                }

                if (0 == filterCriteriaCnt) throw new ArgumentException("Need at least one address criterion");

                IQuery q = _session.CreateQuery(qs.ToString());

                if (country != null) q = q.SetString("co", country);
                if (postalCode != null) q = q.SetString("po", postalCode);
                if (state != null) q = q.SetString("stpr", state);
                if (municipality != null) q = q.SetString("mu", municipality);
                if (addressFreeText != null) q = q.SetString("fa", addressFreeText.Replace('*', '%'));
                    
                return q.List<Building>();
            }
        }

		public IList<Building> SearchByProximity(double longitude, double latitude,
			double dLon, double dLat)
		{
			lock (_session)
			{
				IQuery q = _session.CreateQuery("FROM Vre.Server.BusinessLogic.Building WHERE "
					+ "Deleted=false AND Location.Longitude>:lonLeft AND Location.Longitude<:lonRight "
					+ "AND Location.Latitude>:latLeft AND Location.Latitude<:latRight");

				q.SetDouble("lonLeft", longitude - dLon);
				q.SetDouble("lonRight", longitude + dLon);
				q.SetDouble("latLeft", latitude - dLat);
				q.SetDouble("latRight", latitude + dLat);

				return q.List<Building>();
			}
		}
    }
}