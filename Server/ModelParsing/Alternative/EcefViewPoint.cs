using System;

namespace Vre.Server.Model.Kmz
{
    /// <summary>
    /// Cartesian Earth-Centric Earth-Fixed coordinate
    /// </summary>
    public class EcefViewPoint
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

        /// <summary>
        /// In meters
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// In meters
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// In meters
        /// </summary>
        public double Z { get; set; }
        public double Ax { get; set; }
        public double Ay { get; set; }
        public double Az { get; set; }

        public ViewPoint Base { get; private set; }

        public EcefViewPoint(EcefViewPoint copy)
        {
            //Base = new ViewPoint(copy.AsViewPoint());

            //X = 0.0;
            //Y = 0.0;
            //Z = 0.0;
            //Ax = 0.0;
            //Ay = 0.0;
            //Az = 0.0;

            Base = new ViewPoint(copy.Base);

            X = copy.X;
            Y = copy.Y;
            Z = copy.Z;
            Ax = copy.Ax;
            Ay = copy.Ay;
            Az = copy.Az;
        }

        public EcefViewPoint(ViewPoint source)
        {
            Base = new ViewPoint(source);

            X = 0.0;
            Y = 0.0;
            Z = 0.0;
            Ax = 0.0;
            Ay = 0.0;
            Az = 0.0;

            //double lonRad = source.Longitude * DegreesToRad;
            //double latRad = source.Latitude * DegreesToRad;

            //double lonCos = Math.Cos(lonRad);
            //double latCos = Math.Cos(latRad);
            //double lonSin = Math.Sin(lonRad);
            //double latSin = Math.Sin(latRad);

            //double Rn = EarthSemimajorAxisM / Math.Sqrt(1.0 - (EarthExSquared * latSin * latSin));

            //X = (Rn + source.Altitude) * latCos * lonCos;
            //Y = (Rn + source.Altitude) * latCos * lonSin;
            //Z = (Rn * (1.0 - EarthExSquared) + source.Altitude) * latSin;

            //Ax = latRad * lonSin
            //    - (source.Heading * DegreesToRad) * latCos * lonCos
            //    - (source.Roll * DegreesToRad) * latSin * lonCos
            //    - (source.Tilt * DegreesToRad) * lonSin;

            //Ay = Math.PI / 2.0 + latRad * lonCos
            //    + (source.Heading * DegreesToRad) * latCos * lonSin
            //    - (source.Roll * DegreesToRad) * latSin * lonSin
            //    - (source.Tilt * DegreesToRad) * lonCos;

            //Az = -lonRad
            //    + (source.Heading * DegreesToRad) * latSin * lonSin
            //    + (source.Roll * DegreesToRad) * latCos;
        }

#if DEBUG
        public ViewPoint DbgVp { get { return AsViewPoint(); } }
#endif

        public ViewPoint AsViewPoint()
        {
            // converting relative Cartesian coordinates to geographical using base geographical point

            ViewPoint result = new ViewPoint(Base);

            // base point coordinates conversion
            double latSin = Math.Sin(Base.Latitude * DegreesToRad);
            double latCos = Math.Cos(Base.Latitude * DegreesToRad);

            double hdgSin = Math.Sin(Base.Heading * DegreesToRad);
            double hdgCos = Math.Cos(Base.Heading * DegreesToRad);

            // relative coordinate translation
            double lonDx = X * hdgCos
                - Y * hdgSin
                - Z * Math.Sin(Base.Tilt * DegreesToRad);

            double latDx = Y * hdgCos
                - X * hdgSin
                + Z * Math.Sin(Base.Roll * DegreesToRad);

            double altDx = Z * Math.Cos(Base.Tilt * DegreesToRad) * Math.Cos(Base.Roll * DegreesToRad)
                + X * Math.Sin(Base.Tilt * DegreesToRad)
                - Y * Math.Sin(Base.Roll * DegreesToRad);

            // calculating long/lat/alt
            result.Altitude = result.Altitude + altDx;
            double Rn = EarthSemimajorAxisM / Math.Sqrt(1.0 - (EarthExSquared * latSin * latSin));
            result.Latitude = result.Latitude + Math.Asin(latDx / Rn) * RadToDegrees;
            Rn = Rn * latCos;
            result.Longitude = result.Longitude + lonDx / ((Rn * Math.PI * 2.0) / 360.0);
            
            // calculating viewpoint angles
            double xSin = Math.Sin(Ax);
            double xCos = Math.Cos(Ax);
            double ySin = Math.Sin(Ay);
            double yCos = Math.Cos(Ay);
            double zSin = Math.Sin(Az);
            double zCos = Math.Cos(Az);

            double hdg = degAngleFrom3DComponents(Base.Heading,
                Az * xCos * yCos,
                -Ax * ySin,
                Ay * xSin);

            double tilt = degAngleFrom3DComponents(Base.Tilt,
                -Ay * xCos * zCos,
                Ax * zSin,
                Az * xSin);

            double roll = degAngleFrom3DComponents(Base.Roll,
                Ax * yCos * zCos,
                -Ay * zSin,
                Az * ySin);

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
    }
}