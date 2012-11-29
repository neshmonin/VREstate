using System.Collections.Generic;
using System.Xml;

namespace Vre.Server.Model.Kmz
{
    public class Geometry
    {
        public class Point3D
        {
            public double X;
            public double Y;
            public double Z;

            public Point3D(double x, double y, double z)
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

        public IEnumerable<Point3D> Points { get { return _points; } }
        public IEnumerable<Line> Lines { get { return _lines; } }
        public string Id { get; private set; }

        private Point3D[] _points;
        private Line[] _lines;

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

        public Geometry(string id, XmlNode root)
        {
            Id = id;
            _points = null;
            _lines = null;

            XmlNode meshNode = root["mesh"];
            if (meshNode != null)
            {
                XmlNode pn = meshNode["source"];
                XmlNode array = (pn != null) ? pn["float_array"] : null;

                // TODO: read these nodes to verify point array format!
                //XmlNode techNode = pn["technique_common"];
                //XmlNode accessorNode = (techNode != null) ? techNode["accessor"] : null;

                if (array != null)
                {
                    XmlAttribute aa = array.Attributes["count"];
                    int cnt = int.Parse(aa.Value);
                    _points = new Point3D[cnt / 3];
                    string[] parts = array.InnerText.Split(null);
                    for (int idx = _points.Length - 1, pos = parts.Length - 3; idx >= 0; idx--, pos -= 3)
                        _points[idx] = new Point3D(
                            double.Parse(parts[pos]),
                            double.Parse(parts[pos + 1]),
                            double.Parse(parts[pos + 2]));
                }

                XmlNode ln = meshNode["lines"];
                array = (ln != null) ? ln["p"] : null;

                if (array != null)
                {
                    XmlAttribute la = ln.Attributes["count"];
                    int cnt = int.Parse(la.Value);
                    _lines = new Line[cnt];
                    string[] parts = array.InnerText.Split(null);
                    for (int idx = _lines.Length - 1, pos = parts.Length - 2; idx >= 0; idx--, pos -= 2)
                        _lines[idx] = new Line(
                            int.Parse(parts[pos]),
                            int.Parse(parts[pos + 1]));
                }
            }
        }
    }
}
