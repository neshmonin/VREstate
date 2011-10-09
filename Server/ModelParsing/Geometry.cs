using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace VrEstate
{
    public class Geometry
    {
        public class Point3
        {
            public double X;
            public double Y;
            public double Z;
            public Point3(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }

        public class Line
        {
            public int Start;
            public int End;
            public Line(int start, int end)
            {
                Start = start;
                End = end;
            }
        }

        public Point3[] Points;
        public List<Line> Lines = new List<Line>();

        //<geometry id="ID28531">
        //    <mesh>
        //        <source id="ID28532">
        //            <float_array id="ID28534" count="39">
        //              -68.71062 -62.4295 95.1932
        //              -52.06116 -62.43301 95.1932
        //              -68.66311 162.8034 95.1932
        //              -68.66311 162.8034 88.51298
        //              -68.66311 162.7888 88.51298
        //              -68.66838 162.8034 -6.012202
        //              0.03434296 162.7889 -6.012202
        //              0.03434296 162.7889 -6.680225
        //              -0.0415789 -197.0879 -6.680225
        //              -52.08956 -197.0769 -6.680225
        //              0.03434297 162.7889 30.22802
        //              -52.08906 -194.6832 95.1932
        //              -52.08906 -194.6832 88.51298
        //            </float_array>
        //            <technique_common>
        //                <accessor count="13" source="#ID28534" stride="3">
        //                    <param name="X" type="float" />
        //                    <param name="Y" type="float" />
        //                    <param name="Z" type="float" />
        //                </accessor>
        //            </technique_common>
        //        </source>
        //        <vertices id="ID28533">
        //            <input semantic="POSITION" source="#ID28532" />
        //        </vertices>
        //        <lines count="12" material="Material2">
        //            <input offset="0" semantic="VERTEX" source="#ID28533" />
        //            <p>1 0 2 0 3 2 4 3 4 5 5 6 7 6 8 7 8 9 10 6 1 11 11 12</p>
        //        </lines>
        //    </mesh>
        //</geometry>
        public Geometry(XmlElement geometryElt)
        {
            foreach (var geomChildNode in geometryElt.FirstChild.ChildNodes)
            {
                XmlElement geomChild = geomChildNode as XmlElement;
                int count = 0;
                switch (geomChild.Name)
                {
                    case "source":
                        foreach (var sourceChildNode in geomChild.ChildNodes)
                        {
                            XmlElement float_array = sourceChildNode as XmlElement;
                            if (float_array != null && float_array.Name == "float_array")
                            {
                                count = int.Parse(float_array.GetAttribute("count"));
                                Points = new Point3[count/3];
                                string [] floats = float_array.InnerText.Split(null);
                                int indx = 0;
                                for (int i = 0; i < count; i=i+3)
                                {
                                    double x = double.Parse(floats[i]);
                                    double y = double.Parse(floats[i+1]);
                                    double z = double.Parse(floats[i+2]);

                                    Points[indx] = new Point3(x, y, z);
                                    indx++;
                                }
                                break;
                            }
                        }
                        break;
                    case "vertices":
                        break;
                    case "lines":
                        count = int.Parse(geomChild.GetAttribute("count"));
                        foreach (var linesChildNode in geomChild.ChildNodes)
                        {
                            XmlElement p = linesChildNode as XmlElement;
                            if (p != null && p.Name == "p")
                            {
                                string[] indxs = p.InnerText.Split(null);
                                for (int i = 0; i < count; i = i + 2)
                                {
                                    int start = int.Parse(indxs[i]);
                                    int end = int.Parse(indxs[i + 1]);

                                    Lines.Add(new Line(start, end));
                                }
                                break;
                            }
                        }
                        break;
                } // switch
            }
        } // Geometry constructor

        public delegate void AddLineDelegete(double xStart, double yStart, double zStart,
                                             double xEnd,   double yEnd,   double zEnd);
        public void DrawAll(AddLineDelegete addLineDelegate)
        {
            foreach (var line in Lines)
            {
                double xStart = Points[line.Start].X;
                double yStart = Points[line.Start].Y;
                double zStart = Points[line.Start].Z;
                double xEnd = Points[line.End].X;
                double yEnd = Points[line.End].Y;
                double zEnd = Points[line.End].Z;
                addLineDelegate(xStart, yStart, zStart, xEnd, yEnd, zEnd);
            }
        }
    }
}
