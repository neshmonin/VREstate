using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Vre.Server.Model.Kmz
{
    public class ConstructionSite
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<Building> Buildings { get { return _buildings; } }
        public EcefViewPoint LocationCart { get; private set; }
        /// <summary>
        /// Fall-back: direct value read from model file.
        /// </summary>
        public ViewPoint LocationGeo { get; private set; }
        public Dictionary<string, Geometry[]> Geometries { get { return _geometries; } }
        public double UnitInMeters { get; private set; }

        private List<Building> _buildings;
        private Dictionary<string, Geometry[]> _geometries;

        public ConstructionSite(Model parent, string name, XmlNode rootNode,
            Dictionary<string, XmlNode> models,
            XmlNode geometryRoot, TMatrix tMatrix)
        {
            UnitInMeters = parent.UnitInMeters;
            Name = name;
            LocationCart = new EcefViewPoint(parent.Location);
            LocationGeo = new ViewPoint(parent.Location);
            _buildings = new List<Building>();

            XmlAttribute na = rootNode.Attributes["id"];
            if (na != null) Id = na.Value;

            XmlNode sketchUpNode = null;
            foreach (XmlNode n in rootNode.ChildNodes)
            {
                if (!n.Name.Equals("node")) continue;

                na = n.Attributes["name"];
                if (null == na) continue;

                if (na.Value.Equals("SketchUp")) { sketchUpNode = n; break; }
            }
            if (null == sketchUpNode) throw new InvalidDataException("scene node does not contain a SketchUp subnode in Collada structure");
            
            foreach (XmlNode n in sketchUpNode.ChildNodes)
            {
                XmlNode nn;
                XmlAttribute nna;
                string nodeId = null;

                nn = n["instance_node"];
                if (nn != null)
                {
                    nna = nn.Attributes["url"];
                    if (nna != null)
                    {
                        nodeId = nna.Value;
                        if (nodeId.StartsWith("#")) nodeId = nodeId.Substring(1);
                    }
                }

                XmlNode buildingNode = null;
                if (nodeId != null) if (!models.TryGetValue(nodeId, out buildingNode)) buildingNode = null;

                nn = n["matrix"];
                nna = n.Attributes["name"];

                if ((buildingNode != null) && (nn != null) && (nna != null))
                {
                    TMatrix matrix = new TMatrix(tMatrix, nn.InnerText, UnitInMeters);
                    string buildingName = nna.Value.Replace('_', ' ');

                    _buildings.Add(new Building(this, nodeId, buildingName, buildingNode, models, matrix));
                }
            }

            //
            // Build a dictionary of <suite class name>-<geometry>
            //
            _geometries = new Dictionary<string, Geometry[]>();

            // build a list of used geometries
            List<string> usedGeometryIds = new List<string>();
            foreach (Building b in _buildings)
            {
                foreach (Suite s in b.Suites)
                {
                    foreach (string id in s.GeometryIdList)
                        if (!usedGeometryIds.Contains(id)) usedGeometryIds.Add(id);
                }
            }

            // build a dictionary with all related geometries
            Dictionary<string, Geometry> geometries = new Dictionary<string, Geometry>(usedGeometryIds.Count);
            foreach (XmlNode gn in geometryRoot.ChildNodes)
            {
                if (!gn.Name.Equals("geometry")) continue;

                XmlAttribute gna = gn.Attributes["id"];
                if (null == gna) continue;

                if (usedGeometryIds.Contains(gna.Value))
                    geometries.Add(gna.Value, new Geometry(gn));
            }

            // build class-geometry dictionary
            foreach (Building b in _buildings)
            {
                foreach (Suite s in b.Suites)
                {
                    if (_geometries.ContainsKey(s.ClassName)) continue;

                    IEnumerable<string> gidl = s.GeometryIdList;
                    List<Geometry> gl = new List<Geometry>();
                    foreach (string id in gidl)
                    {
                        Geometry g;
                        if (geometries.TryGetValue(id, out g)) gl.Add(g);
                    }

                    _geometries.Add(s.ClassName, gl.ToArray());
                }
            }
        }
    }
}