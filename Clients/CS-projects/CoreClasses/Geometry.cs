using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CoreClasses
{
    public class Geometry : Vre.Server.BusinessLogic.Wireframe
    {
        protected List<Point3D> Points
        {
            get { return _points; }
            set { _points = value; }
        }
        protected List<Segment> Segments
        {
            get { return _segments; }
            set { _segments = value; }
        }

        public Geometry() : base(new List<Point3D>(), new List<Segment>())
        {
        }

        public delegate void AddLineDelegete(double latStart, double lonStart, double altStart,
                                             double latEnd, double lonEnd, double altEnd);

        public void DrawAll(AddLineDelegete addLineDelegate)
        {
            foreach (var line in Segments)
            {
                double xStart = Points[line.StartIdx].X;
                double yStart = Points[line.StartIdx].Y;
                double zStart = Points[line.StartIdx].Z;
                double xEnd = Points[line.EndIdx].X;
                double yEnd = Points[line.EndIdx].Y;
                double zEnd = Points[line.EndIdx].Z;
                addLineDelegate(xStart, yStart, zStart, xEnd, yEnd, zEnd);
            }
        }
    }
}
