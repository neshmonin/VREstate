using System;
using System.Collections.Generic;
using System.Xml;

namespace VrEstate
{
    public class Site : CoreClasses.ICountable
    {
        public const double i2m = 0.0254;

        private string m_name;
        private string m_dirName;

        private int m_id = 108514320;
        private Dictionary<string, Building> m_buildings = new Dictionary<string, Building>();
        private Dictionary<string, Geometry[]> m_geometries = new Dictionary<string, Geometry[]>();
        private double m_unitInMeters;
        public enum MeasureScale { Meters, Inches };
        public static MeasureScale m_scale = MeasureScale.Inches;

        private double m_Lon_d = 0;
        private double m_Lat_d = 0;
        private double m_Alt_m = 0;
        public static double LonModel_d { get { return Model.Lon_d; } }
        public static double LatModel_d { get { return Model.Lat_d; } }

        public Site(XmlDocument xmlDoc)
        {
            m_name = Model.ReadableName; 
            XmlElement asset_Node = xmlDoc.GetElementsByTagName("asset").Item(0) as XmlElement;
            foreach (var childNode in asset_Node.ChildNodes)
            {
                XmlElement elt = childNode as XmlElement;
                if (elt == null)
                    continue;

                switch (elt.Name)
                {
                    case "contributor":
                        break;
                    case "created":
                        break;
                    case "modified":
                        break;
                    case "unit":
                        //Assert.IsTrue(elt.HasAttributes);
                        m_unitInMeters = Math.Round(double.Parse(elt.GetAttribute("meter")),2);
                        
                        // TODO: make sure the model in Merets has this value 1.0
                        if (m_unitInMeters == Math.Round(0.0254d, 2))
                            m_scale = MeasureScale.Inches;
                        else
                            m_scale = MeasureScale.Meters;

                        break;
                    case "up_axis":
                        //Assert.IsTrue(elt.InnerText == "Z_UP");
                        break;
                }
            }

            XmlElement visual_scene_Node = xmlDoc.GetElementsByTagName("visual_scene").Item(0) as XmlElement;
            XmlElement sketchUp_node = visual_scene_Node.FirstChild as XmlElement;
            XmlElement library_nodes_Node = xmlDoc.GetElementsByTagName("library_nodes").Item(0) as XmlElement;
            XmlElement library_geometries_Node = xmlDoc.GetElementsByTagName("library_geometries").Item(0) as XmlElement;

            //Dictionary<string, string> buildingLibNodeIds = new Dictionary<string, string>();

            //collectBuildingLibNodeIds(sketchUp_node, buildingLibNodeIds);

            foreach (var childNode in sketchUp_node.ChildNodes)
            {
                XmlElement elt = childNode as XmlElement;
                if (elt == null)
                    continue;

                if (elt.Name != "node")
                    continue;

                string name = elt.GetAttribute("name");
                //if (!name.StartsWith(Model.InstPrefix))
                //    continue;

                Building building = Building.Create(elt, library_nodes_Node, library_geometries_Node);
                if (building != null)
                {
                    m_Lon_d += building.Lon_d;
                    m_Lat_d += building.Lat_d;
                    m_Alt_m += building.Alt_m;

                    m_buildings.Add(building.Name, building);
                }
            }
            // average up the Lon-s/Lat-s/Alt-s - to obtain the coordinates of the site center
            m_Lon_d = m_Lon_d / m_buildings.Count;
            m_Lat_d = m_Lat_d / m_buildings.Count;
            m_Alt_m = m_Alt_m / m_buildings.Count;

            //
            // Build a dictionary of <suite class name>-<geometry>
            //

            // build a list of used geometries
            List<string> usedGeometryIds = new List<string>();
            foreach (var bldg in m_buildings.Values)
            {
                foreach (var suite in bldg.Suites.Values)
                {
                    foreach (string id in suite.GeometryIdList)
                        if (!usedGeometryIds.Contains(id)) usedGeometryIds.Add(id);
                }
            }

            // build a dictionary with all related geometries
            Dictionary<string, Geometry> geometries = new Dictionary<string, Geometry>(usedGeometryIds.Count);
            foreach (var geom in library_geometries_Node.ChildNodes)
            {
                XmlElement geomelt = geom as XmlElement;
                if (geomelt != null && geomelt.Name == "geometry")
                {
                    string geomId = geomelt.GetAttribute("id");

                    if (usedGeometryIds.Contains(geomId))
                        geometries.Add(geomId, new Geometry(geomelt));
                }
            }

            // build class-geometry dictionary
            foreach (var bldg in m_buildings.Values)
            {
                foreach (var suite in bldg.Suites.Values)
                {
                    if (m_geometries.ContainsKey(suite.ClassId)) continue;

                    string[] gidl = suite.GeometryIdList;
                    List<Geometry> gl = new List<Geometry>(gidl.Length);
                    foreach (string id in gidl)
                    {
                        Geometry g;
                        if (geometries.TryGetValue(id, out g)) gl.Add(g);
                    }

                    m_geometries.Add(suite.ClassId, gl.ToArray());
                }
            }
        }

        // TODO: UNUSED
        private static void collectBuildingLibNodeIds(XmlElement sketchUp_node, Dictionary<string, string> buildingNodes)
        {
            string buildingNodePrefix = "Building_";

            foreach (var childNode in sketchUp_node.ChildNodes)
            {
                XmlElement elt = childNode as XmlElement;
                if (elt == null)
                    continue;

                if (elt.Name != "node")
                    continue;

                string name = elt.GetAttribute("name");

                if (name.StartsWith(buildingNodePrefix))
                {
                    foreach (var subnode in elt.ChildNodes)
                    {
                        XmlElement selt = subnode as XmlElement;
                        if (selt == null)
                            continue;

                        if (selt.Name != "instance_node")
                            continue;

                        buildingNodes.Add(name.Substring(buildingNodePrefix.Length), selt.GetAttribute("url"));
                    }
                }
            }
        }

        public Suite FindSuiteByName(string suiteName)
        {
            Suite suite = null;
            foreach (var building in Buildings.Values)
            {
                suite = building.FindSuiteByName(suiteName);
                if (suite != null)
                    return suite;
            }
            return null;
        }

        public int ID
        {
            get { return m_id; }
        }

        public Dictionary<string, Building> Buildings
        {
            get { return m_buildings; }
        }

        /// <summary>
        /// (suite class name)-(geometry)
        /// </summary>
        public Dictionary<string, Geometry[]> Geometries
        {
            get { return m_geometries; }
        }

        public double Lon_d
        {
            get { return m_Lon_d; }
        }

        public double Lat_d
        {
            get { return m_Lat_d; }
        }

        public double Alt_m
        {
            get { return m_Alt_m; }
        }

        public int HowManyItems
        {
            get
            {
                int totalPlacemarks = 0;
                foreach (var building in Buildings.Values)
                    totalPlacemarks += building.HowManyItems;

                return totalPlacemarks;
            }
        }

        public string Name { get { return m_name; } }
        public string DirName
        {
            get { return m_dirName; }
            set { m_dirName = value; }
        }

        public static MeasureScale Scale
        {
            get { return m_scale; }
            set { m_scale = value; }
        }

        public void Save(ref XmlDocument xmlDoc)
        {
            XmlElement library_nodes_Node = xmlDoc.GetElementsByTagName("library_nodes").Item(0) as XmlElement;
            foreach (var bldng in m_buildings.Values)
            {
                foreach (var childNode in library_nodes_Node.ChildNodes)
                {
                    XmlElement elt = childNode as XmlElement;

                    if (elt == null)
                        continue;

                    if (elt.Name != "node")
                        continue;

                    if (elt.GetAttribute("id") != bldng.BuildingId)
                        continue;

                    bldng.Save(ref elt);
                    break;
                }
            }
        }

        public static double EarthRadiusAtLat
        {
            get
            {
                if (m_scale == MeasureScale.Meters)
                    return Model.EarthRadiusAtLat;

                return Model.EarthRadiusAtLat / i2m;
            }
        }

        public static double EarthRadius
        {
            get
            {
                if (m_scale == MeasureScale.Meters)
                    return Model.EarthRadius;

                return Model.EarthRadius / i2m;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
