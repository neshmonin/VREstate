using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Vre.Server.Model.Kmz
{
    public class ConstructionSite
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<Building> Buildings { get { return _buildings; } }
		public IEnumerable<Structure> Structures { get { return _structures; } }
		public EcefViewPoint LocationCart { get; private set; }
        /// <summary>
        /// Fall-back: direct value read from model file.
        /// </summary>
        public ViewPoint LocationGeo { get; private set; }
        public Dictionary<string, Geometry[]> Geometries { get { return _geometries; } }
        public double UnitInMeters { get; private set; }

        private List<Building> _buildings;
		private List<Structure> _structures;
		private Dictionary<string, Geometry[]> _geometries;
        internal Model _model;

        public ConstructionSite(Model parent, string name, XmlNode rootNode,
            Dictionary<string, XmlNode> models,
            XmlNode geometryRoot, TMatrix tMatrix, StringBuilder readWarnings)
        {
            StringBuilder fatalErrors = new StringBuilder();
            _model = parent;
            UnitInMeters = parent.UnitInMeters;
            Name = name;
            LocationCart = new EcefViewPoint(parent.Location);
            LocationGeo = new ViewPoint(parent.Location);
            _buildings = new List<Building>();
			_structures = new List<Structure>();

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
            if (null == sketchUpNode) throw new InvalidDataException("MDSC08: Scene node does not contain a SketchUp subnode in Collada structure");
            
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

                XmlNode unitNode = null;
                string buildingTypeName = string.Empty;
				string structureTypeName = string.Empty;
				if (nodeId != null)
                {
                    if (!models.TryGetValue(nodeId, out unitNode))
                    {
                        unitNode = null;
                    }
                    else
                    {
                        var unitTypeName = unitNode.Attributes["name"].Value;
                        if (unitTypeName.StartsWith(Building.XmlPrefix))
                        {
                            buildingTypeName = unitTypeName.Substring(Building.XmlPrefix.Length);
                            if (buildingTypeName.Contains("_"))
                                fatalErrors.AppendFormat("\r\nBuilding type name '{0}' contains underscores.", 
                                    unitNode.Attributes["name"].Value);
                        }
						else if (unitTypeName.StartsWith(Structure.XmlPrefix))
						{
							structureTypeName = unitTypeName.Substring(Structure.XmlPrefix.Length);
							if (structureTypeName.Contains("_"))
								fatalErrors.AppendFormat("\r\nStructure type name '{0}' contains underscores.",
									unitNode.Attributes["name"].Value);
						}
						else
                        {
                            if (readWarnings != null)
                                readWarnings.AppendFormat(
                                    "\r\nMDSC05: Found a component which definition name (\'{0}\') does not begin with \'_building_\' keyword.",
                                    buildingTypeName);

                            buildingTypeName = string.Empty;
                        }
                    }

                }

                nn = n["matrix"];
                nna = n.Attributes["name"];

                if ((unitNode != null) && (nn != null) && (nna != null))
                {
                    TMatrix matrix = new TMatrix(tMatrix, nn.InnerText, UnitInMeters);
                    string unitName = nna.Value.Replace('_', ' ').Trim();
                    try
                    {
						if (!string.IsNullOrEmpty(buildingTypeName))
							_buildings.Add(new Building(this, nodeId, unitName, buildingTypeName, unitNode, models, matrix));
						else if (!string.IsNullOrEmpty(structureTypeName))
							_structures.Add(new Structure(this, nodeId, unitName, buildingTypeName, unitNode, models, matrix));
					}
                    catch (InvalidDataException ide)
                    {
                        fatalErrors.AppendFormat("\r\n{0}", ide.Message);
                    }
                }
            }

            if ((0 == _buildings.Count) && (0 == _structures.Count))
                throw new InvalidDataException("MDSC07: No '_building_' and no '_structure_' components found in KMZ.");
            if (fatalErrors.Length > 0)
                throw new InvalidDataException(fatalErrors.ToString());

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
                {
                    Geometry geom = Geometry.Create(gna.Value, gn);
                    if (geom != null)
                    {
                        if ((geom.Points != null) || (geom.Lines != null))  // skip unknown data
                            geometries.Add(gna.Value, geom);
                    }
                    //else
                    //{
                    //    if (readWarnings != null)
                    //        readWarnings.AppendFormat(
                    //            "\r\nMDSC06: Suite Class Name \'{0}\': Wire geometry contains no lines", gna.Value);
                    //}
                }
            }

            // build class-geometry dictionary
            foreach (Building b in _buildings)
            {
                foreach (Suite s in b.Suites)
                {
                    string fullType = s.ClassName.Contains("/") ? s.ClassName : b.Type + "/" + s.ClassName;
                    if (_geometries.ContainsKey(fullType)) continue;

                    IEnumerable<string> gidl = s.GeometryIdList;
                    List<Geometry> gl = new List<Geometry>();
                    foreach (string id in gidl)
                    {
                        Geometry g;
                        if (geometries.TryGetValue(id, out g)) gl.Add(g);
                    }

                    _geometries.Add(fullType, gl.ToArray());
                }
            }
        }
    }
}