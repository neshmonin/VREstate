using System.Globalization;
using System.Xml;

namespace Vre.Server.Model.Kmz
{
    public class GeoPoint
    {
        /// <summary>
        /// In degrees -180...180; Eastern hemisphere is positive.
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// In degrees -90...90; North hemisphere is positive.
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// In meters above ground level.
        /// </summary>
        public double Altitude { get; set; }

        /// <summary>
        /// True if object contains no data.
        /// </summary>
        public bool IsEmpty { get { return (Longitude > 190.0); } }
        /// <summary>
        /// True if object represents a point not on the ground level (Altitude is non-zero).
        /// </summary>
        public bool Is3D { get { return (Altitude != 0.0); } }

        public GeoPoint() { Longitude = 200.0; }

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

        public GeoPoint(XmlNode root)
        {
            const string LongitudeNodeName = "longitude";
            const string LatitudeNodeName = "latitude";
            const string AltitudeNodeName = "altitude";

            XmlNode node = root.FirstChild;
            while (node != null)
            {
                if (node.Name.Equals(LongitudeNodeName)) Longitude = double.Parse(node.InnerText, CultureInfo.InvariantCulture);
                else if (node.Name.Equals(LatitudeNodeName)) Latitude = double.Parse(node.InnerText, CultureInfo.InvariantCulture);
                else if (node.Name.Equals(AltitudeNodeName)) Altitude = double.Parse(node.InnerText, CultureInfo.InvariantCulture);

                node = node.NextSibling;
            }
        }
    }
}