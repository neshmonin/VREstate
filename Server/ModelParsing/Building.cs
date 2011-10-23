using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
//using NUnit.Framework;

namespace VrEstate
{
    public class Building : Model, CoreClasses.ICountable
    {
        public const string XmlPrefix = "_building_";

        private int m_id = 108514319;
        private Dictionary<string, Suite> m_suites = new Dictionary<string, Suite>();

        private double m_Lon_d = 0;
        private double m_Lat_d = 0;
        private double m_Alt_m = 0;
        private double m_MaxAlt_m = 0;
        private string m_buildingId;
        private string m_buildingName;
        private double[] m_LonLatAlt;
        public double LonModel_d { get { return m_LonLatAlt[0]; } }
        public double LatModel_d { get { return m_LonLatAlt[1]; } }
        public double AltModel_m
        {
            get
            {
                if (Site.Scale == Site.MeasureScale.Meters)
                    return m_LonLatAlt[2];
                else
                    return m_LonLatAlt[2] * Site.i2m;
            }
        }

        public static Building Create(XmlElement xmlElt, XmlElement library_nodes_Node, XmlElement library_geometries_Node)
        {
            string buildingId = "";
            double[][] matrix = Model.ExtractMatrix_InstanceNodeURL(xmlElt, out buildingId);

            foreach (var childNode in library_nodes_Node.ChildNodes)
            {
                XmlElement elt = childNode as XmlElement;

                if (elt == null)
                    continue;

                if (elt.Name != "node")
                    continue;

                if (elt.GetAttribute("id") != buildingId)
                    continue;

                // if we got here, then elt is the node that contains references to
                // all Suites in the Building
                string buildingName = elt.GetAttribute("name");
                if (buildingName.StartsWith(Building.XmlPrefix))
                {
                    string nameCandidate = xmlElt.GetAttribute("name");
                    if (!nameCandidate.StartsWith(Model.InstPrefix))
                        buildingName = nameCandidate.Replace('_', ' ');

                    return new Building(buildingId, buildingName,
                                        elt, library_nodes_Node, library_geometries_Node,
                                        matrix);
                }
                break;
            }

            return null;
        }

        protected Building(string buildingId,
                           string buildingName, 
                           XmlElement buildingNode,
                           XmlElement library_nodes_Node,
                           XmlElement library_geometries_Node,
                           double[][] matrix )
        {
            m_buildingId  = buildingId;
            m_buildingName= buildingName;
            m_LonLatAlt = XYZ2LonLatAlt(matrix[0][3], matrix[1][3], matrix[2][3]);

            foreach (var suiteElt in buildingNode.ChildNodes)
            {
                XmlElement elt = suiteElt as XmlElement;

                if (elt == null)
                    continue;

                if (elt.Name != "node")
                    continue;

                string name = elt.GetAttribute("name");
                if (name.StartsWith("instance_"))
                    continue;

                Suite suite = new Suite(elt, library_nodes_Node, library_geometries_Node, matrix);
                m_Lon_d += suite.Lon_d;
                m_Lat_d += suite.Lat_d;
                m_Alt_m += suite.Alt_m;

                if (m_MaxAlt_m < suite.Alt_m)
                    m_MaxAlt_m = suite.Alt_m;

                //if (m_suites.ContainsKey(name))
                //    m_suites.Equals(name);
                //else
                    m_suites.Add(name, suite);
            }

            // average up the Lon-s/Lat-s/Alt-s - to obtain the coordinates of the building center
            m_Lon_d = m_Lon_d / m_suites.Count;
            m_Lat_d = m_Lat_d / m_suites.Count;
            m_Alt_m = m_Alt_m / m_suites.Count;

        }

        public string Name
        {
            get { return m_buildingName; }
            set { m_buildingName = value; }
        }

        public string BuildingId
        {
            get { return m_buildingId; }
            set { m_buildingId = value; }
        }

        public int ID
        {
            get { return m_id; }
        }

        public Dictionary<string, Suite> Suites
        {
            get { return m_suites; }
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
            get
            {
                //if (Site.Scale == Site.MeasureScale.Meters)
                    return m_Alt_m;
                //else
                //    return m_Alt_m * Site.i2m;
            }
        }

        public double MaxAlt_m
        {
            get { return m_MaxAlt_m; }
        }

        public int HowManyItems
        {
            get
            {
                int totalPlacemarks = 1;
                foreach (var suite in m_suites.Values)
                    totalPlacemarks += suite.HowManyItems;

                return totalPlacemarks;
            }
        }

        public void GenerateBuildingInfoFile(string filename)
        {
            string content = string.Format("var g_buildingInfo = {{\n" +
                                           "    id: {0},\n" +
                                           "    numOfSuites: {1},\n" +
                                           "    lon: {2},\n    lat: {3},\n    alt: {4},\n" +
                                           "}};\n",
                                            ID,
                                            m_suites.Count,
                                            m_Lon_d, m_Lat_d, m_Alt_m
                                           );
            // Create the file and write to it.
            System.IO.StreamWriter writer;
            writer = System.IO.File.CreateText(filename);
            writer.WriteLine(content);
            writer.Close();
        }

        public override string ToString()
        {
            return Name;
        }

        public int HowMany(Suite.SaleStatus stat)
        {
            int res = 0;
            foreach (var suite in m_suites.Values)
                if (suite.Status == stat) res++;

            return res;
        }

        public void Save(ref XmlElement buildingNode)
        {
            foreach (var suiteElt in buildingNode.ChildNodes)
            {
                XmlElement elt = suiteElt as XmlElement;

                if (elt == null)
                    continue;

                if (elt.Name != "node")
                    continue;

                string name = elt.GetAttribute("name");
                if (name.StartsWith("instance_"))
                    continue;

                Suite suite = m_suites[name];
                suite.Save(ref elt);
            }
        }
        public Suite FindSuiteByName(string suiteName)
        {
            if (Suites.ContainsKey(suiteName))
                return Suites[suiteName];

            return null;
        }
    }
}

