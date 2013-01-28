using System;

namespace Vre.Server.Model.Kmz
{
    /// <summary>
    /// Cartesian Earth-Centric Earth-Fixed coordinate
    /// </summary>
    public class EcefViewPoint
    {
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

        public ViewPoint AsViewPoint() { return GeoUtilities.EcefViewPointAsViewPoint(this); }
    }
}