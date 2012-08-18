using System;

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
                    _matrix[i][j] = result[i][j];
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
                        double x = double.Parse(ee);

                        if (i > 3) throw new FormatException("Matrix string is invalid (2)");  // extra values

                        if (0 == j) _matrix[i] = new double[4];

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

        //public void GetGeographicCoordinates(out double lon, out double lat, out double alt)
        //{
        //    double x = _matrix[0][2] * (-_earthRm) + _matrix[0][3];
        //    double y = _matrix[1][2] * (-_earthRm) + _matrix[1][3];
        //    double z = _matrix[2][2] * (-_earthRm) + _matrix[2][3];
        //    double w = _matrix[3][2] * (-_earthRm) + _matrix[3][3];
        //    lon = 0.0;
        //    lat = 0.0;
        //    alt = 0.0;
        //}

        public EcefViewPoint Transform(EcefViewPoint vp)
        {
            const double baseAngle = Math.PI / 4.0;
            const double fullCircle = Math.PI * 2.0;
            // http://www.physicsforums.com/showthread.php?t=293302
            // http://www.blancmange.info/notes/maths/vectors/mops/
            EcefViewPoint result = new EcefViewPoint(vp);

            //if (IsLocationTransform)
            {
                result.X = /*_matrix[0][0] * vp.X + _matrix[1][0] * vp.Y + _matrix[2][0] * vp.Z +*/ vp.X + _matrix[0][3] * _matrixUnitInMeters;
                result.Y = /*_matrix[0][1] * vp.X + _matrix[1][1] * vp.Y + _matrix[2][1] * vp.Z +*/ vp.Y + _matrix[1][3] * _matrixUnitInMeters;
                result.Z = /*_matrix[0][2] * vp.X + _matrix[1][2] * vp.Y + _matrix[2][2] * vp.Z +*/ vp.Z + _matrix[2][3] * _matrixUnitInMeters;

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


                //dx = _matrix[0][0] + _matrix[1][0] + _matrix[2][0];
                //dy = _matrix[0][1] + _matrix[1][1] + _matrix[2][1];
                //dz = _matrix[0][2] + _matrix[1][2] + _matrix[2][2];
                ////double dx = _matrix[0][0] + _matrix[0][1] + _matrix[0][2];
                ////double dy = _matrix[1][0] + _matrix[1][1] + _matrix[1][2];
                ////double dz = _matrix[2][0] + _matrix[2][1] + _matrix[2][2];

                ////result.Ax += baseAngle - Math.Atan2(dz, dy);
                ////result.Ay += baseAngle - Math.Atan2(dz, dx);
                ////result.Az += baseAngle - Math.Atan2(dy, dx);

                ////if (isZero(_matrix[0][2]) && isZero(_matrix[1][2]) && isZero(_matrix[2][0]) && isZero(_matrix[2][1]) && isOne(_matrix[2][2]))
                ////{   // rotation around Z
                ////    result.Az += Math.Acos(_matrix[0][0]);
                ////}
                ////else
                ////{
                ////    result.Ax = vp.Ax + Math.Acos(dx); if (result.Ax > Math.PI) result.Ax -= fullCircle;
                ////    result.Ay = vp.Ay + Math.Acos(dy); if (result.Ay > Math.PI) result.Ay -= fullCircle;
                ////    result.Az = vp.Az + Math.Acos(dz); if (result.Az > Math.PI) result.Az -= fullCircle;
                ////}
            }
            //else throw new InvalidOperationException("Don't know how to use the matrix for such transformation");

            return result;
        }
    }
}