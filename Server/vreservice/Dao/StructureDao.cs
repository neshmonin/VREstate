using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    public class StructureDao : UpdateableBaseDao<Structure>
    {
        public StructureDao(ISession session) : base(session) { }

		public Structure GetById(string id)
		{
			lock (_session)
			{
				int key;
				if (int.TryParse(id, out key)) return GetById(key);

				return _session.CreateCriteria<Structure>()
					.Add(Restrictions.Eq("Name", id))
					.Add(Restrictions.Eq("Deleted", false))
					.UniqueResult<Structure>();
			}
		}

		public IList<Structure> SearchByProximity(double longitude, double latitude,
			double dLon, double dLat)
		{
			lock (_session)
			{
				IQuery q = _session.CreateQuery("FROM Vre.Server.BusinessLogic.Structure WHERE "
					+ "Deleted=false AND Location.Longitude>:lonLeft AND Location.Longitude<:lonRight "
					+ "AND Location.Latitude>:latLeft AND Location.Latitude<:latRight");

				q.SetDouble("lonLeft", longitude - dLon);
				q.SetDouble("lonRight", longitude + dLon);
				q.SetDouble("latLeft", latitude - dLat);
				q.SetDouble("latRight", latitude + dLat);

				return q.List<Structure>();
			}
		}
    }
}