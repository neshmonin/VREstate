using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    /// <summary>
    /// Geometry list in model's terminology
    /// </summary>
    [Serializable]
    public class Wireframe : IClientDataProvider
    {
        public struct Point3D
        {
            public double X, Y, Z;
            public Point3D(double x, double y, double z) { X = x; Y = y; Z = z; }
        }

        public struct Segment
        {
            public int StartIdx, EndIdx;
            public Segment(int startIdx, int endIdx) { StartIdx = startIdx; EndIdx = endIdx; }
        }

        protected List<Point3D> _points;
        protected List<Segment> _segments;

        public Wireframe(List<Point3D> points, List<Segment> segments) 
        { 
            _points = points; 
            _segments = segments; 
        }

        public ClientData GetClientData()
        {
            ClientData result = new ClientData();

            double[] points = new double[_points.Count * 3];
            int idx = 0;
            foreach (Point3D p in _points)
            {
                points[idx++] = p.X;
                points[idx++] = p.Y;
                points[idx++] = p.Z;
            }
            result.Add("points", points);

            int[] segments = new int[_segments.Count * 2];
            idx = 0;
            foreach (Segment s in _segments)
            {
                segments[idx++] = s.StartIdx;
                segments[idx++] = s.EndIdx;
            }
            result.Add("lines", segments);  // keep model's terminology

            return result;
        }

        //public class WireframeCluster
        //{
        //    public List<Point3D> Points;
        //    public List<Segment> Segments;
        //    public WireframeCluster(List<Point3D> points, List<Segment> segments) { Points = points; Segments = segments; }
        //}

        //protected List<WireframeCluster> _clusters;

        //public Wireframe(List<WireframeCluster> clusters)
        //{
        //    _clusters = clusters;
        //}

        //public ClientData GetClientData()
        //{
        //    ClientData[] wfcs = new ClientData[_clusters.Count];
        //    int wfcidx = 0;

        //    foreach (WireframeCluster wfc in _clusters)
        //    {
        //        ClientData cd = new ClientData();

        //        double[] points = new double[wfc.Points.Count * 3];
        //        int idx = 0;
        //        foreach (Point3D p in wfc.Points)
        //        {
        //            points[idx++] = p.X;
        //            points[idx++] = p.Y;
        //            points[idx++] = p.Z;
        //        }
        //        cd.Add("points", points);

        //        int[] segments = new int[wfc.Segments.Count * 2];
        //        idx = 0;
        //        foreach (Segment s in wfc.Segments)
        //        {
        //            segments[idx++] = s.StartIdx;
        //            segments[idx++] = s.EndIdx;
        //        }
        //        cd.Add("lines", segments);  // keep model terminology

        //        wfcs[wfcidx++] = cd;
        //    }

        //    ClientData result = new ClientData();
        //    result.Add("geometries", wfcs);
        //    return result;
        //}

        public bool UpdateFromClient(ClientData data)
        {
            throw new NotImplementedException();
        }
    }
}
