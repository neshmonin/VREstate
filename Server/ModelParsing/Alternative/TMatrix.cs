using System;
using System.Globalization;

namespace Vre.Server.Model.Kmz
{
    public class TMatrix
    {
        //public enum TransType { Unknown, 
        //    /// <summary>
        //    /// Shift: linear move
        //    /// </summary>
        //    Translation, 
        //    /// <summary>
        //    /// Scale coordinates
        //    /// </summary>
        //    Scale, 
        //    /// <summary>
        //    /// Rotation around X
        //    /// </summary>
        //    Pitch, 
        //    /// <summary>
        //    /// Rotation around Z
        //    /// </summary>
        //    Roll, 
        //    /// <summary>
        //    /// Rotation around Y
        //    /// </summary>
        //    Yaw }
        ////private const double _earthRm = 6378135.0;

        //public TransType MatrixType { get; private set; }

        private double[][] _matrix;
        private double _matrixUnitInMeters;

        /// <summary>
        /// We use this constructor to create a TMatrix object corresponding to a unity vector rotated
        /// around Z-axis to heading_d degrees
        /// </summary>
        /// <param name="heading_d"></param>
        public TMatrix(double heading_d)
        {
            _matrix = new double[4][];
            double heading_r = DegreesToRadians(heading_d);
            //           | cos(h)  sin(h)   0     0|
            // XYZd(h) = |-sin(h)  cos(h)   0     0|
            //           |   0       0      1     0|
            //           |   0       0      0     1|
            _matrix[0] = new double[4];
            _matrix[0][0] = Math.Cos(heading_r); _matrix[0][1] = Math.Sin(heading_r); _matrix[0][2] = 0; _matrix[0][3] = 0; 

            _matrix[1] = new double[4];
            _matrix[1][0] = -Math.Sin(heading_r); _matrix[1][1] = Math.Cos(heading_r); _matrix[1][2] = 0; _matrix[1][3] = 0;

            _matrix[2] = new double[4];
            _matrix[2][0] = 0; _matrix[2][1] = 0; _matrix[2][2] = 1; _matrix[2][3] = 0;

            _matrix[3] = new double[4];
            _matrix[3][0] = 0; _matrix[3][1] = 0; _matrix[3][2] = 0; _matrix[3][3] = 1;
        }

        public TMatrix(TMatrix baseMatrix, string colladaEncodedString, double matrixUnitInMeters)
        {
            init(colladaEncodedString, matrixUnitInMeters);

            double[][] result = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                result[i] = new double[4];

                for (int j = 0; j < 4; j++)
                {
                    result[i][j] = 0.0;
                    for (int n = 0; n < 4; n++)
                        result[i][j] += baseMatrix._matrix[i][n] * _matrix[n][j];
                }
            }

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    if (i < 3 && j < 3)
                    {
                        if (result[i][j] > 1) result[i][j] = 1;
                        if (result[i][j] < -1) result[i][j] = -1;
                    }
                    _matrix[i][j] = result[i][j];
                }
        }

        public TMatrix(string colladaEncodedString, double matrixUnitInMeters)
        {
            init(colladaEncodedString, matrixUnitInMeters);
        }

        private void init(string colladaEncodedString, double matrixUnitInMeters)
        {
            _matrixUnitInMeters = matrixUnitInMeters;
            _matrix = new double[4][];

            string[] e0 = colladaEncodedString.Split(' ');
            if (e0.Length < 16) throw new FormatException("Matrix string is invalid (1)");  // too short text array

            int i = 0, j = 0;
            try
            {
                foreach (string e in e0)
                {
                    string ee = e.Trim();
                    if (ee.Length > 0)
                    {
                        double x = double.Parse(ee, CultureInfo.InvariantCulture);

                        if (i > 3) throw new FormatException("Matrix string is invalid (2)");  // extra values

                        if (0 == j) _matrix[i] = new double[4];

                        if (i < 3 && j < 3)
                        {
                            if (x > 1) x = 1;
                            if (x < -1) x = -1;
                        }
                        _matrix[i][j] = x;

                        if (++j > 3) { j = 0; i++; }
                    }
                }
            }
            catch (FormatException)
            {
                throw new FormatException("Matrix string is invalid (3)");  // one of values is not an int/float
            }

            if ((i != 4) || (j != 0)) throw new FormatException("Matrix string is invalid (4)");  // not enough read values

            if (!isZero(_matrix[3][0]) || !isZero(_matrix[3][1]) || !isZero(_matrix[3][2]) || !isOne(_matrix[3][3]))
                throw new FormatException("Matrix string is invalid (5)");  // not a proper transformation matrix

            //// http://ruthless.zathras.de/facts/apps/polygonesia/3d-transformation-matrix.php
            //if (isOne(_matrix[0][0]) && isZero(_matrix[0][1]) && isZero(_matrix[0][2])// && isZero(_matrix[0][3])
            //    && isZero(_matrix[1][0]) && isOne(_matrix[1][1]) && isZero(_matrix[1][2])// && isZero(_matrix[1][3])
            //    && isZero(_matrix[2][0]) && isZero(_matrix[2][1]) && isOne(_matrix[2][2])// && isZero(_matrix[2][3])
            //    && isZero(_matrix[3][0]) && isZero(_matrix[3][1]) && isZero(_matrix[3][2]) && isOne(_matrix[3][3]))
            //    MatrixType = TransType.Translation;
            //else if (isZero(_matrix[0][1]) && isZero(_matrix[0][2])// && isZero(_matrix[0][3])
            //    && isZero(_matrix[1][0]) && isZero(_matrix[1][2])// && isZero(_matrix[1][3])
            //    && isZero(_matrix[2][0]) && isZero(_matrix[2][1])// && isZero(_matrix[2][3])
            //    && isZero(_matrix[3][0]) && isZero(_matrix[3][1]) && isZero(_matrix[3][2]) && isOne(_matrix[3][3]))
            //    MatrixType = TransType.Scale;
            //else if (isOne(_matrix[0][0]) && isZero(_matrix[0][1]) && isZero(_matrix[0][2])// && isZero(_matrix[0][3])
            //    && isZero(_matrix[1][0])// && isZero(_matrix[1][3])
            //    && isZero(_matrix[2][0])// && isZero(_matrix[2][3])
            //    && isZero(_matrix[3][0]) && isZero(_matrix[3][1]) && isZero(_matrix[3][2]) && isOne(_matrix[3][3]))
            //    MatrixType = TransType.Pitch;
            //else if (isZero(_matrix[0][1])// && isZero(_matrix[0][3])
            //    && isZero(_matrix[1][0]) && isOne(_matrix[1][1]) && isZero(_matrix[1][2])// && isZero(_matrix[1][3])
            //    && isZero(_matrix[2][1])// && isZero(_matrix[2][3])
            //    && isZero(_matrix[3][0]) && isZero(_matrix[3][1]) && isZero(_matrix[3][2]) && isOne(_matrix[3][3]))
            //    MatrixType = TransType.Yaw;
            //else if (isZero(_matrix[0][2])// && isZero(_matrix[0][3])
            //    && isZero(_matrix[1][2])// && isZero(_matrix[1][3])
            //    && isZero(_matrix[2][0]) && isZero(_matrix[2][1]) && isOne(_matrix[2][2])// && isZero(_matrix[2][3])
            //    && isZero(_matrix[3][0]) && isZero(_matrix[3][1]) && isZero(_matrix[3][2]) && isOne(_matrix[3][3]))
            //    MatrixType = TransType.Roll;
            //else
            //    MatrixType = TransType.Unknown;


            //System.Diagnostics.Debug.Assert(MatrixType != TransType.Unknown);
        }

        private static bool isZero(double x) { return (Math.Abs(x) <= 1E-12/*double.Epsilon*/); }

        private static bool isOne(double x) { return (Math.Abs(Math.Abs(x) - 1.0) <= double.Epsilon); }

        /* I still do not understand this code... */
        public EcefViewPoint Transform(EcefViewPoint vp)
        {
            const double baseAngle = Math.PI / 4.0;
            EcefViewPoint result = new EcefViewPoint(vp);

            result.X = vp.X + _matrix[0][3] * _matrixUnitInMeters;
            result.Y = vp.Y + _matrix[1][3] * _matrixUnitInMeters;
            result.Z = vp.Z + _matrix[2][3] * _matrixUnitInMeters;

            double dx, dy, dz;

            dy = _matrix[1][1] + _matrix[2][1];
            dz = _matrix[1][2] + _matrix[2][2];
            result.Ax += baseAngle - Math.Atan2(dz, dy);

            dx = _matrix[0][0] + _matrix[2][0];
            dz = _matrix[0][2] + _matrix[2][2];
            result.Ay += baseAngle - Math.Atan2(dz, dx);

            dx = _matrix[0][0] + _matrix[1][0];
            dy = _matrix[0][1] + _matrix[1][1];
            result.Az += baseAngle - Math.Atan2(dy, dx);

            return result;
        }

        /// <summary>
        /// Extraction of heading angle (in degrees) from the TMatrix object
        /// </summary>
        public double Heading_d
        {
            get
            {
                double angle_d = 0.0f;
                double heading0 = Math.Round(RadiansToDegrees(Math.Acos(Math.Round(_matrix[0][0], 3))), 2);
                double heading1 = Math.Round(RadiansToDegrees(Math.Asin(Math.Round(_matrix[1][0], 3))), 2);
                if (heading0 > 0 == heading1 > 0)
                {
                    if (Math.Sign(heading0) == Math.Sign(heading1))
                    {   //    N A  * red
                        //      | /
                        //      |/ 
                        //  <---+----->
                        //  W   |     E
                        //    S V
                        angle_d = - heading0;
                    }
                    else
                    {   //    N A
                        //      |
                        //  <---+----->
                        //  W   |\    E
                        //      | \
                        //    S V  * red
                        angle_d = heading0;
                    }
                }
                else
                {
                    if (Math.Sign(heading0) == Math.Sign(heading1))
                    {   // red *  A N
                        //      \ |
                        //       \|
                        //  <-----+--->
                        //  W     |   E
                        //      S V
                        angle_d = heading1 + 180;
                    }
                    else
                    {   //        A N
                        //        |
                        //  <-----+--->
                        //  W    /|   E
                        //      / |
                        // red *  V S
                        angle_d = heading0;
                    }
                }
                return angle_d;
            }
        }

        /// <summary>
        /// Patched heading calculation to parry client's (v1.0.0.17) heading usage issue.
        /// </summary>
        public double Heading_d_patch
        {
            get
            {
                double angle_d = 0.0f;
                double heading0 = Math.Round(RadiansToDegrees(Math.Acos(Math.Round(_matrix[0][0], 3))), 2);
                double heading1 = Math.Round(RadiansToDegrees(Math.Asin(Math.Round(_matrix[1][0], 3))), 2);
                if (heading0 > 0 == heading1 > 0)
                {
                    if (Math.Sign(heading0) == Math.Sign(heading1))
                    {   //    N A  * red
                        //      | /
                        //      |/ 
                        //  <---+----->
                        //  W   |     E
                        //    S V
                        angle_d = (90 - heading0);
                    }
                    else
                    {   //    N A
                        //      |
                        //  <---+----->
                        //  W   |\    E
                        //      | \
                        //    S V  * red
                        angle_d = (90 + heading0);
                    }
                }
                else
                {
                    if (Math.Sign(heading0) == Math.Sign(heading1))
                    {   // red *  A N
                        //      \ |
                        //       \|
                        //  <-----+--->
                        //  W     |   E
                        //      S V
                        angle_d = heading1 + 270;
                    }
                    else
                    {   //        A N
                        //        |
                        //  <-----+--->
                        //  W    /|   E
                        //      / |
                        // red *  V S
                        angle_d = heading0 + 90;
                    }
                }
                return angle_d;
            }
        }

        /// <summary>
        /// This is a generic transform procedure - to rotate+translate a given Point3D with
        /// the TMatrix object.
        /// TBD: we need also to address caling here!
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Geometry.Point3D Transform(Geometry.Point3D point)
        {
            return Translate(Rotate(point));
        }

        /// <summary>
        /// Apply rotation to the given Point3D
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Geometry.Point3D Rotate(Geometry.Point3D point)
        {
            Geometry.Point3D res = new Geometry.Point3D(0, 0, 0);
            double rotation = DegreesToRadians(Heading_d);

            res.X = point.X * Math.Cos(rotation) + point.Y * Math.Sin(rotation);
            res.Y = -point.X * Math.Sin(rotation) + point.Y * Math.Cos(rotation);
            res.Z = point.Z;

            return res;
        }

        /// <summary>
        /// Apply translation to the given Point3D
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Geometry.Point3D Translate(Geometry.Point3D point)
        {
            Geometry.Point3D res = new Geometry.Point3D(0, 0, 0);
            /* translation */
            res.X = (point.X + _matrix[0][3]) * _matrixUnitInMeters;
            res.Y = (point.Y + _matrix[1][3]) * _matrixUnitInMeters;
            res.Z = (point.Z + _matrix[2][3]) * _matrixUnitInMeters;

            return res;
        }

        protected const double earthR_m = 6378135;
        /// <summary>
        /// Convert from Decart {X, Y, Z} to Polar (Lat, Lon, Alt) coordinates
        /// </summary>
        /// <param name="point"></param>
        /// <param name="Location"></param>
        /// <returns></returns>
        public ViewPoint Point3D2ViewPoint(Geometry.Point3D point, ViewPoint Location)
        {
            double EarthRadiusAtLat = earthR_m * Math.Cos(DegreesToRadians(Location.Latitude));

            ViewPoint vp = new ViewPoint();
            vp.Longitude = Location.Longitude + RadiansToDegrees(Math.Atan(point.X / EarthRadiusAtLat));
            vp.Latitude = Location.Latitude + RadiansToDegrees(Math.Atan(point.Y / earthR_m));
            vp.Altitude = point.Z;

            return vp;
        }

        /// <summary>
        /// Conversion from Degrees to Radians
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static double DegreesToRadians(double degrees)
        {
            return (Math.PI / 180d) * degrees;
        }

        /// <summary>
        /// Conversion from Radians to Degrees
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static double RadiansToDegrees(double radians)
        {
            return (180d / Math.PI) * radians;
        }

    }
}