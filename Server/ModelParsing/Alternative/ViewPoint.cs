using System.Globalization;
using System.Xml;

namespace Vre.Server.Model.Kmz
{
    public class ViewPoint : GeoPoint
    {
        /// <summary>
        /// In degrees 0...360 starting from North clock-wise.
        /// </summary>
        public double Heading { get; set; }
        /// <summary>
        /// In degrees 0...90 starting from horizon level AKA Elevation.
        /// </summary>
        public double Tilt { get; set; }
        /// <summary>
        /// In degrees -180...180 starting from zenith; right direction is positive.
        /// </summary>
        public double Roll { get; set; }

        //public ViewPoint(double longitude, double latitude, double altitude,
        //    double horizontalHeading, double verticalHeading)
        //    : base(longitude, latitude, altitude)
        //{
        //    HorizontalHeading = horizontalHeading;
        //    VerticalHeading = verticalHeading;
        //    Roll = 0.0;
        //}

        //public ViewPoint(double longitude, double latitude, double altitude,
        //    double horizontalHeading)
        //    : base(longitude, latitude, altitude)
        //{
        //    HorizontalHeading = horizontalHeading;
        //    VerticalHeading = 0.0;
        //}

        //public ViewPoint(double longitude, double latitude, double horizontalHeading)
        //    : base(longitude, latitude)
        //{
        //    HorizontalHeading = horizontalHeading;
        //    VerticalHeading = 0.0;
        //}

        //public ViewPoint(GeoPoint location,
        //    double horizontalHeading, double verticalHeading)
        //    : base(location.Longitude, location.Latitude, location.Altitude)
        //{
        //    HorizontalHeading = horizontalHeading;
        //    VerticalHeading = verticalHeading;
        //}

        //public ViewPoint(GeoPoint location,
        //    double horizontalHeading)
        //    : base(location.Longitude, location.Latitude, location.Altitude)
        //{
        //    HorizontalHeading = horizontalHeading;
        //    VerticalHeading = 0.0;
        //}

        public ViewPoint() : base() { }

        public ViewPoint(GeoPoint location, XmlNode root)
            : base(location.Longitude, location.Latitude, location.Altitude)
        {
            const string HeadingNodeName = "heading";
            const string TiltNodeName = "tilt";
            const string RollNodeName = "roll";

            XmlNode node = root.FirstChild;
            while (node != null)
            {
                if (node.Name.Equals(HeadingNodeName)) Heading = double.Parse(node.InnerText, CultureInfo.InvariantCulture);
                else if (node.Name.Equals(TiltNodeName)) Tilt = double.Parse(node.InnerText, CultureInfo.InvariantCulture);
                else if (node.Name.Equals(RollNodeName)) Roll = double.Parse(node.InnerText, CultureInfo.InvariantCulture);

                node = node.NextSibling;
            }

        }

        public ViewPoint(ViewPoint copy) 
            : base(copy.Longitude, copy.Latitude, copy.Altitude) 
        {
            Heading = copy.Heading;
            Tilt = copy.Tilt;
            Roll = copy.Roll;
        }
    }
}