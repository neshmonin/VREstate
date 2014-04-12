using System;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;

namespace Vre.Server
{
	internal static class Extensions
	{
		public static Vre.Server.BusinessLogic.ViewPoint AsViewPoint(this Vre.Server.Model.Kmz.ViewPoint vp)
		{
			return new BusinessLogic.ViewPoint(vp.Longitude, vp.Latitude, vp.Altitude, vp.Heading);
		}

		public static Vre.Server.BusinessLogic.GeoPoint AsGeoPoint(this Vre.Server.Model.Kmz.ViewPoint vp)
		{
			return new BusinessLogic.GeoPoint(vp.Longitude, vp.Latitude, vp.Altitude);
		}

		public static Vre.Server.BusinessLogic.GeoPoint AsGeoPoint(this Vre.Server.Model.Kmz.EcefViewPoint evp)
		{
			var vp = evp.AsViewPoint();
			return new BusinessLogic.GeoPoint(vp.Longitude, vp.Latitude, vp.Altitude);
		}

		public static UpdateableBase GetTarget(this ViewOrder vo, ISession session)
		{
			switch (vo.TargetObjectType)
			{
				case ViewOrder.SubjectType.Building:
					using (var dao = new BuildingDao(session))
						return dao.GetById(vo.TargetObjectId);

				case ViewOrder.SubjectType.Suite:
					using (var dao = new SuiteDao(session))
						return dao.GetById(vo.TargetObjectId);

				default:
					return null;
			}
		}

		public static bool IsEqual(this Vre.Server.BusinessLogic.GeoPoint first, Vre.Server.BusinessLogic.GeoPoint second, double maxDistanceErrorM)
		{
			if (maxDistanceErrorM > 5000.0) throw new ArgumentException("Max supported error is 5 km");  // otherwise need more complex formula
			double mx = GeoUtilities.LongitudeDegreeInM(first.Latitude);
			double my = GeoUtilities.LatitudeDegreeInM(first.Latitude);
			double dlon = first.Longitude - second.Longitude;
			double dlat = first.Latitude - second.Latitude;
			return Math.Sqrt(dlon * dlon + dlat * dlat) <= maxDistanceErrorM;
		}

	}
}