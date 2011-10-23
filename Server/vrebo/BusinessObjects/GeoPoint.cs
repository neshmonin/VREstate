using System;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public class GeoPoint : IClientDataProvider
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Altitude { get; set; }

        public bool IsEmpty { get { return ((0.0 == Longitude) && (0.0 == Latitude)); } }
        public bool Is3D { get { return (Altitude != 0.0); } }

        protected GeoPoint() { }

        public GeoPoint(double longitude, double latitude, double altitude)
        {
            Longitude = longitude;
            Latitude = latitude;
            Altitude = altitude;
        }

        public GeoPoint(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
            Altitude = 0.0;
        }

        public ClientData GetClientData()
        {
            ClientData result = new ClientData();

            result.Add("lon", Longitude);
            result.Add("lat", Latitude);
            result.Add("alt", Altitude);

            return result;
        }

        public bool UpdateFromClient(ClientData data)
        {
            bool changed = false;

            Longitude = data.UpdateProperty("lon", Longitude, ref changed);
            Latitude = data.UpdateProperty("lat", Latitude, ref changed);
            Altitude = data.UpdateProperty("alt", Altitude, ref changed);

            return changed;
        }

        public static GeoPoint Empty { get { return new GeoPoint(0.0, 0.0); } }
    }
}