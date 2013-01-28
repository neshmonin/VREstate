using System;
using Vre.Server.Model.Kmz;

namespace Vre.Server
{
    /// <summary>
    /// Geographical coordinate tools
    /// </summary>
    public class GeoUtilities
    {
        /// <summary>
        /// Earth semi-major axis (a) in meters (WGS 84)
        /// <seealso cref="http://en.wikipedia.org/wiki/World_Geodetic_System#A_new_World_Geodetic_System:_WGS_84"/>
        /// </summary>
        public const double EarthSemimajorAxisM = 6378137.0;
        /// <summary>
        /// Earth semi-minor axis (b) in meters (WGS 84)
        /// <seealso cref="http://en.wikipedia.org/wiki/World_Geodetic_System#A_new_World_Geodetic_System:_WGS_84"/>
        /// </summary>
        public const double EarthSemiminorAxisM = 6356752.314245;
        /// <summary>
        /// Earth eccentricity ((major ^2 - minor ^ 2) / major ^ 2) (WGS 84)
        /// <seealso cref="http://en.wikipedia.org/wiki/Longitude#Length_of_a_degree_of_longitude"/>
        /// </summary>
        public const double EarthEccentricity = ((EarthSemimajorAxisM * EarthSemimajorAxisM) - (EarthSemiminorAxisM * EarthSemiminorAxisM)) / 
            (EarthSemimajorAxisM * EarthSemimajorAxisM);
        /// <summary>
        /// Earth inverse flattening (1/f), (major / (major - minor)) (WGS 84)
        /// <seealso cref="http://en.wikipedia.org/wiki/World_Geodetic_System#A_new_World_Geodetic_System:_WGS_84"/>
        /// </summary>
        public const double EarthInverseFlattening = 298.257223563;
        /// <summary>
        /// Square of the first numerical eccentricity of the Earth ellipsoid (calculated)
        /// </summary>
        public const double EarthExSquared = 1.0 - ((EarthSemiminorAxisM * EarthSemiminorAxisM) / (EarthSemimajorAxisM * EarthSemimajorAxisM));
        /// <summary>
        /// Coefficient to convert degrees to radians
        /// </summary>
        public const double DegreesToRad = Math.PI / 180.0;
        /// <summary>
        /// Coefficient to convert radians to degrees
        /// </summary>
        public const double RadToDegrees = 180.0 / Math.PI;

        public static ViewPoint EcefViewPointAsViewPoint(EcefViewPoint evp)
        {
            // converting relative Cartesian coordinates to geographical using base geographical point

            ViewPoint result = new ViewPoint(evp.Base);

            // base point coordinates conversion
            double latSin = Math.Sin(evp.Base.Latitude * DegreesToRad);
            double latCos = Math.Cos(evp.Base.Latitude * DegreesToRad);

            double hdgSin = Math.Sin(evp.Base.Heading * DegreesToRad);
            double hdgCos = Math.Cos(evp.Base.Heading * DegreesToRad);

            // relative coordinate translation
            double lonDx = evp.X * hdgCos
                - evp.Y * hdgSin
                - evp.Z * Math.Sin(evp.Base.Tilt * DegreesToRad);

            double latDx = evp.Y * hdgCos
                - evp.X * hdgSin
                + evp.Z * Math.Sin(evp.Base.Roll * DegreesToRad);

            double altDx = evp.Z * Math.Cos(evp.Base.Tilt * DegreesToRad) * Math.Cos(evp.Base.Roll * DegreesToRad)
                + evp.X * Math.Sin(evp.Base.Tilt * DegreesToRad)
                - evp.Y * Math.Sin(evp.Base.Roll * DegreesToRad);

            // calculating long/lat/alt
            result.Altitude = result.Altitude + altDx;
            double Rn = EarthSemimajorAxisM / Math.Sqrt(1.0 - (EarthExSquared * latSin * latSin));
            result.Latitude = result.Latitude + Math.Asin(latDx / Rn) * RadToDegrees;
            Rn = Rn * latCos;
            result.Longitude = result.Longitude + lonDx / ((Rn * Math.PI * 2.0) / 360.0);
            
            // calculating viewpoint angles
            double xSin = Math.Sin(evp.Ax);
            double xCos = Math.Cos(evp.Ax);
            double ySin = Math.Sin(evp.Ay);
            double yCos = Math.Cos(evp.Ay);
            double zSin = Math.Sin(evp.Az);
            double zCos = Math.Cos(evp.Az);

            double hdg = degAngleFrom3DComponents(evp.Base.Heading,
                evp.Az * xCos * yCos,
                -evp.Ax * ySin,
                evp.Ay * xSin);

            double tilt = degAngleFrom3DComponents(evp.Base.Tilt,
                -evp.Ay * xCos * zCos,
                evp.Ax * zSin,
                evp.Az * xSin);

            double roll = degAngleFrom3DComponents(evp.Base.Roll,
                evp.Ax * yCos * zCos,
                -evp.Ay * zSin,
                evp.Az * ySin);

            result.Heading = result.Heading + hdg;
            if (result.Heading >= 360.0) result.Heading = result.Heading - 360.0;

            result.Tilt = result.Tilt + tilt;
            if (result.Tilt > 180.0) result.Tilt = result.Tilt - 360.0;

            result.Roll = result.Roll + roll;
            if (result.Roll > 180.0) result.Roll = result.Roll - 360.0;


            // Classic Earth-centric ECEF to Geographical coordinates translation
            //
            //double lon = Math.Atan2(Y, X);

            //double p = Math.Sqrt(X * X + Y * Y);
            //double Zp = Z / p;
            //double lat2 = Math.Atan(Zp);

            //double alt, lat;

            //do
            //{
            //    lat = lat2;
            //    double latSin = Math.Sin(lat);
            //    double Rn = EarthSemimajorAxisM / Math.Sqrt(1.0 - (EarthExSquared * latSin * latSin));
            //    alt = (p / Math.Cos(lat)) - Rn;
            //    lat2 = Math.Atan2(Zp, 1.0 - EarthExSquared * (Rn / (Rn + alt)));
            //}
            //while (Math.Abs(lat - lat2) > 0.000000001);  // this is an empiric value to achieve required precision



            //ViewPoint result = new ViewPoint();

            //result.Longitude = lon * RadToDegrees;
            //result.Latitude = lat * RadToDegrees;
            //result.Altitude = alt;

            //double xSin = Math.Sin(Ax);
            //double xCos = Math.Cos(Ax);
            //double ySin = Math.Sin(Ay);
            //double yCos = Math.Cos(Ay);
            //double zSin = Math.Sin(Az);
            //double zCos = Math.Cos(Az);

            //result.Heading = degAngleFrom3DComponents(
            //    -Ax * zCos * yCos,
            //    -Ay * zSin * xCos,
            //    Az * xSin * ySin);

            //result.Roll = degAngleFrom3DComponents(
            //    -Az * xCos * yCos,
            //    Ay * xSin,
            //    -Ax * ySin);

            //result.Tilt = degAngleFrom3DComponents(
            //    -Ay * xCos * zCos,
            //    - Ax * zSin * yCos,
            //    1.0);

            ////result.Tilt = Tilt * RadToDegrees;
            ////result.Roll = Roll * RadToDegrees;
            ////result.Heading = Pitch * RadToDegrees;

            return result;
        }

        private static double degAngleFrom3DComponents(double cx, double cy, double cz)
        {
            return Math.Sqrt(cx * cx + cy * cy + cz * cz) * RadToDegrees;
        }

        private static double degAngleFrom3DComponents(double baseDeg, double cx, double cy, double cz)
        {
            double result = baseDeg + Math.Sqrt(cx * cx + cy * cy + cz * cz) * RadToDegrees;
            if (result > 180.0) result -= 360.0;
            return result;
        }

        /// <summary>
        /// Calculate length of one degree of longitude in meters for a given latitude.
        /// </summary>
        /// <param name="latitude">Latitude in degrees</param>
        /// <returns>Length of one degree of longitude in meters</returns>
        public static double LongitudeDegreeInM(double latitude)
        {
            double lat = latitude * DegreesToRad;
            return 111412.84 * Math.Cos(lat) - 93.5 * Math.Cos(lat * 3.0)
                + 0.118 * Math.Cos(lat * 5.0);
        }

        /// <summary>
        /// Calculate length of one degree of latitude in meters for a given latitude.
        /// </summary>
        /// <param name="latitude">Latitude in degrees</param>
        /// <returns>Length of one degree of latitude in meters</returns>
        public static double LatitudeDegreeInM(double latitude)
        {
            double lat = latitude * DegreesToRad;
            return 111132.954 - 559.822 * Math.Cos(lat * 2.0)
                + 1.175 * Math.Cos(lat * 4.0) - 0.0023 * Math.Cos(lat * 6.0);
        }
    }
}