using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace VrEstate
{
    public class Model
    {

        private static string m_kmlName;
        private static string m_kmlDescription;

        protected const double earthR_m = 6378135;
        public const String InstPrefix = "instance_";

        protected static double m_roll_r;
        protected static double m_tilt_r;
        protected static double m_heading_r;

        private static double m_lon0;
        public static double Lon_d { get { return Model.m_lon0; } }
        private static double m_lat0;
        public static double Lat_d { get { return Model.m_lat0; } }
        protected static double m_alt0;
        public static double Alt_m { get { return Model.m_alt0; } }

        public static void Setup(XmlDocument xmlDoc)
        {
            XmlElement pm_Node = xmlDoc.GetElementsByTagName("Placemark").Item(0) as XmlElement;
            foreach (var childNode in pm_Node.ChildNodes)
            {
                XmlElement elt = childNode as XmlElement;
                if (elt == null)
                    continue;

                if (elt.Name == "name")
                {   //like: <name>8 Park Road, M4W 3S5</name>
                    m_kmlName = elt.InnerText;
                    break;
                }

                if (elt.Name == "description")
                {
                    m_kmlDescription = elt.InnerText;
                    break;
                }
            }

            XmlElement model_Node = xmlDoc.GetElementsByTagName("Model").Item(0) as XmlElement;
            foreach (var childNode in model_Node.ChildNodes)
            {
                XmlElement elt = childNode as XmlElement;
                if (elt == null)
                    continue;

                if (elt.Name == "Location")
                {
                    foreach (var node in elt.ChildNodes)
                    {
                        XmlElement e = node as XmlElement;
                        if (e == null)
                            continue;

                        switch (e.Name)
                        {
                            case "longitude":
                                m_lon0 = double.Parse(e.InnerText);
                                break;
                            case "latitude":
                                m_lat0 = double.Parse(e.InnerText);
                                break;
                            case "altitude":
                                m_alt0 = double.Parse(e.InnerText);
                                break;
                        }
                    }
                }
                else
                if (elt.Name == "Orientation")
                {
                    foreach (var node in elt.ChildNodes)
                    {
                        XmlElement e = node as XmlElement;
                        if (e == null)
                            continue;

                        switch (e.Name)
                        {
                            case "heading":
                                m_heading_r = Angle.DegreesToRadians(double.Parse(e.InnerText));
                                break;
                            case "tilt":
                                m_tilt_r = Angle.DegreesToRadians(double.Parse(e.InnerText));
                                break;
                            case "roll":
                                m_roll_r = Angle.DegreesToRadians(double.Parse(e.InnerText));
                                break;
                        }
                    }
                }
            }
        }

        public static string ReadableName
        {
            get { return Model.m_kmlName; }
        }

        public static double[] GetRangeHeadingTilt(double deltaLat_d, double deltaLon_d, double deltaAlt_m)
        {
            double deltaLat_r = Angle.DegreesToRadians(deltaLat_d);
            double deltaLon_r = Angle.DegreesToRadians(deltaLon_d);
            double dLat_m = EarthRadius * Math.Tan(deltaLat_r);
            double dLon_m = EarthRadiusAtLat * Math.Tan(deltaLon_r);
            double horizRange_m = Math.Sqrt(dLat_m*dLat_m + dLon_m*dLon_m);
            double range_m = Math.Sqrt(horizRange_m * horizRange_m + deltaAlt_m * deltaAlt_m);
            double heading_d = Angle.RadiansToDegrees(Math.Atan2(dLon_m, dLat_m));
            double tilt_d = 90d - Angle.RadiansToDegrees(Math.Atan2(deltaAlt_m, horizRange_m));

            return new double[] { range_m, heading_d, tilt_d };
        }

        public static double EarthRadius
        {
            get { return earthR_m; }
        }

        public static double EarthRadiusAtLat
        {
            get { return earthR_m * Math.Cos(Angle.DegreesToRadians(m_lat0)); }
        }

        public static double[] getCorrectedXY_m(double[] x)
        {
            var newXY = new double[2];
            newXY[0] = x[0] * Math.Cos(m_heading_r) + x[1] * Math.Sin(m_heading_r);
            newXY[1] = x[0] * Math.Sin(m_heading_r) + x[1] * Math.Cos(m_heading_r);
            return newXY;
        }

        public static double[] ApplyRollTiltHeading(double x, double y, double z)
        {
            var newXYZ = new double[3];
            // http://code.google.com/apis/kml/documentation/kmlreference.html#orientation
            //
            // first - roll (clockwise rotation around Y axis):
            //
            //          | cos(r)  0  sin(r) |
            // XYZ(r) = |   0     1     0   |
            //          | sin(r)  0  cos(r) |
            //
            newXYZ[0] = x * Math.Cos(m_roll_r) + z * Math.Sin(m_roll_r);
            newXYZ[2] = x * Math.Sin(m_roll_r) + z * Math.Cos(m_roll_r);

            // then - tilt (clockwise rotation around X axis):
            //
            //          | 1    0       0    |
            // XYZ(t) = | 0  cos(t)  sin(t) |
            //          | 0  sin(t)  cos(t) |
            //
            newXYZ[1] = y * Math.Cos(m_tilt_r) + newXYZ[2] * Math.Sin(m_tilt_r);
            newXYZ[2] = y * Math.Sin(m_tilt_r) + newXYZ[2] * Math.Cos(m_tilt_r);

            // then - heading (clockwise rotation around Z axis):
            //
            //          | cos(h)  sin(h)  0 |
            // XYZ(h) = |-sin(h)  cos(h)  0 |
            //          |   0       0     1 |
            //
            newXYZ[0] =   newXYZ[0] * Math.Cos(m_heading_r) + newXYZ[1] * Math.Sin(m_heading_r);
            newXYZ[1] = - newXYZ[0] * Math.Sin(m_heading_r) + newXYZ[1] * Math.Cos(m_heading_r);

            return newXYZ;
        }

        public static double determinant(double[][] m)
        {
            double det = m[0][0]*m[1][1]*m[2][2] + m[0][1]*m[1][2]*m[2][0] + m[0][2]*m[1][0]*m[2][1] - 
                         m[0][0]*m[1][2]*m[2][1] - m[0][1]*m[1][0]*m[2][2] - m[0][2]*m[1][1]*m[2][0];

            //double det = m[0][0]*m[1][1]*m[2][2] + m[1][0]*m[2][1]*m[0][2] + m[2][0]*m[0][1]*m[1][2] - 
            //             m[0][0]*m[2][1]*m[1][2] - m[1][0]*m[0][1]*m[2][2] - m[2][0]*m[1][1]*m[0][2];

            return det;
        }

        public static double[] ApplyRollTiltHeadingMatrix(double x, double y, double z, double[][] matrix)
        {
            var newXYZ = new double[3];
            // http://code.google.com/apis/kml/documentation/kmlreference.html#orientation
            //
            // first - roll (clockwise rotation around Y axis):
            //
            //          | cos(r)  0  sin(r) |
            // XYZ(r) = |   0     1     0   |
            //          | sin(r)  0  cos(r) |
            //
            newXYZ[0] = x * matrix[0][0] + z * matrix[0][2];
            newXYZ[2] = x * matrix[2][0] + z * matrix[2][2];

            // then - tilt (clockwise rotation around X axis):
            //
            //          | 1    0       0    |
            // XYZ(t) = | 0  cos(t)  sin(t) |
            //          | 0  sin(t)  cos(t) |
            //
            newXYZ[1] = y * matrix[1][1] + newXYZ[2] * matrix[2][1];
            newXYZ[2] = y * matrix[1][2] + newXYZ[2] * matrix[2][2];

            // then - heading (clockwise rotation around Z axis):
            //
            //          | cos(h) -sin(h)  0 |
            // XYZ(h) = | sin(h)  cos(h)  0 |
            //          |   0       0     1 |
            //
            //newXYZ[0] =   newXYZ[0] * matrix[0][0] + newXYZ[1] * matrix[1][0];
            //newXYZ[1] = - newXYZ[0] * matrix[0][1] + newXYZ[1] * matrix[1][1];

            return newXYZ;
        }

        public static double[] XYZ2LonLatAlt(double x, double y, double z)
        {
            var newXYZ = ApplyRollTiltHeading(x, y, z);
            var lonLatAlt = new double[3];
            lonLatAlt[0] = m_lon0 + Angle.RadiansToDegrees(Math.Atan(newXYZ[0] / Site.EarthRadiusAtLat));
            lonLatAlt[1] = m_lat0 + Angle.RadiansToDegrees(Math.Atan(newXYZ[1] / Site.EarthRadius));
            lonLatAlt[2] = newXYZ[2];

            return lonLatAlt;
        }

        public static double[] XYZ2LonLatAltRelative(double x, double y, double z, double[][] parentMatrix)
        {
            double[] parentLonLatAlt = XYZ2LonLatAlt(parentMatrix[0][3], parentMatrix[1][3], parentMatrix[2][3]);
            var newXYZ = ApplyRollTiltHeading(x, y, z);
            newXYZ = ApplyRollTiltHeadingMatrix(newXYZ[0], newXYZ[1], newXYZ[2], parentMatrix);
            var lonLatAlt = new double[3];
            lonLatAlt[0] = parentLonLatAlt[0] + Angle.RadiansToDegrees(Math.Atan(newXYZ[0] / Site.EarthRadiusAtLat));
            lonLatAlt[1] = parentLonLatAlt[1] + Angle.RadiansToDegrees(Math.Atan(newXYZ[1] / Site.EarthRadius));
            lonLatAlt[2] = newXYZ[2] + parentLonLatAlt[2];

            return lonLatAlt;
        }

        public static double[][] ExtractMatrix_InstanceNodeURL(XmlElement node, out string instanceNodeURL)
        {
            instanceNodeURL = String.Empty;
            double[][] matrix = new double[4][];
            foreach (var childNode in node.ChildNodes)
            {
                XmlElement elt = childNode as XmlElement;
                if (elt.Name == "instance_node")
                    instanceNodeURL = elt.GetAttribute("url").Substring(1);
                else if (elt.Name == "matrix")
                {
                    // matrix is usually a line of 16 float numbers separated with ' ' (space).
                    // it may be some exceptions though - if someone manually edited the COLLADA file,
                    // some extra whitespaces can end up in the split: 
                    string[] elements = elt.InnerText.Split(null);

                    int n = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        matrix[i] = new double[4];
                        for (int j = 0; j < 4; j++)
                        {
                            // Skip all empty elements (if any) that might accure as
                            // a result of the Split operation:
                            while (String.IsNullOrEmpty(elements[n])) { n++; }

                            // Make sure only the parseable elements (i.e. the float numbers)
                            // are ending up in the m_matrix:
                            bool parseSuccess = false;
                            double eltDouble = 0.0;
                            while (parseSuccess == false)
                            {
                                try
                                {
                                    eltDouble = double.Parse(elements[n]);

                                    parseSuccess = true; // found the parseable element!
                                }
                                catch (System.FormatException)
                                {
                                    n++; // skip this element
                                }
                            }
                            matrix[i][j] = eltDouble;
                            n++;
                        }
                    }
                } // elt.Name == "matrix"
            } // foreach childNode in node.ChildNodes

            return matrix;
        }

    }
}
