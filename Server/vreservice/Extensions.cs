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
	}
}