using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
//using NUnit.Framework;

namespace CoreClasses
{
    public class Building : Model, ICSV, CoreClasses.ICountable
    {
        private Vre.Server.BusinessLogic.Building _building;

        public Vre.Server.BusinessLogic.ClientData ClientData
        {
            get { return _building.GetClientData(); }
        }

        public const string XmlPrefix = "_building_";

        private Dictionary<string, Suite> m_suites = new Dictionary<string, Suite>();

        public int ID { get { return _building.AutoID; } }
        private double m_Lon_d = 0;
        private double m_Lat_d = 0;
        private double m_Alt_m = 0;
        private double m_MaxAlt_m = 0;
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

        public Building Create(Vre.Server.BusinessLogic.Building building)
        {
            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                              "suite",
                                                              "building=" + building.AutoID, null);
            if (HttpStatusCode.OK != resp.ResponseCode)
                return null;

            Vre.Server.BusinessLogic.ClientData suiteList = resp.Data;

            foreach (Vre.Server.BusinessLogic.ClientData cd in suiteList.GetNextLevelDataArray("suites"))
            {
                Vre.Server.BusinessLogic.Suite suite = new Vre.Server.BusinessLogic.Suite(cd, _building);
                Suite newSuite = CreateSuite(suite);
                m_Lon_d += suite.Location.Longitude;
                m_Lat_d += suite.Location.Latitude;
                m_Alt_m += suite.Location.Altitude;

                if (m_MaxAlt_m < suite.Location.Altitude)
                    m_MaxAlt_m = suite.Location.Altitude;

                if (!m_suites.ContainsKey(newSuite.UniqueKey))
                    m_suites.Add(newSuite.UniqueKey, newSuite);
            }

            // average up the Lon-s/Lat-s/Alt-s - to obtain the coordinates of the building center
            m_Lon_d = m_Lon_d / m_suites.Count;
            m_Lat_d = m_Lat_d / m_suites.Count;
            m_Alt_m = m_Alt_m / m_suites.Count;
            return this;
        }

        protected virtual Suite CreateSuite(Vre.Server.BusinessLogic.Suite suite)
        {
            return new Suite(suite);
        }

        public Building(Vre.Server.BusinessLogic.Building building)
        {
            _building = building;
        }

        public string ToCSV()
        {
            String outStr = "# ----------------- " + Name + " -----------------,,,,,\n";
            outStr += "#Floor,#Suite,#Type,#Price,#Status,#Panoramic View\n";

            var list = Suites.Values.ToList();
            list.Sort();
            foreach (var suite in list)
            {
                outStr += suite.ToCSV();
            }
            return outStr;
        }

        public bool FromCSV(string csvStream)
        {
            int begin = csvStream.IndexOf("# ----------------- " + Name + " -----------------");
            if (begin == -1) return false;

            csvStream = csvStream.Substring(begin);
            begin = 0;

            int tittleLength = csvStream.IndexOf('\n');
            begin = begin + tittleLength + 1;

            int end = csvStream.IndexOf("# ----------------- ", begin);
            string myCSV;
            if (end != -1)
                myCSV = csvStream.Substring(begin, end - begin);
            else
                myCSV = csvStream.Substring(begin);

            string[] csvLines = myCSV.Split('\n');
            for (int i = 0; i < csvLines.Length; i++)
            {
                if (csvLines[i] == string.Empty)
                    continue;

                if (csvLines[i].StartsWith("#"))
                {
                    csvLines[i] = string.Empty;
                    continue;
                }

                string uniqyeKey = Suite.UniqueKeyFromCSVline(csvLines[i]);
                if (uniqyeKey != "")
                {
                    if (Suites.ContainsKey(uniqyeKey))
                        Suites[uniqyeKey].FromCSV(csvLines[i]);
                    else
                        Trace.WriteLine("CSV line: " + csvLines[i] + ": Cannot recognize a Suite.");

                }
            }

            return true;
        }

        public string Name
        {
            get { return _building.Name; }
            set { _building.Name = value; }
        }

        public Dictionary<string, Suite> Suites
        {
            get { return m_suites; }
        }

        public new double Lon_d
        {
            get { return m_Lon_d; }
        }

        public new double Lat_d
        {
            get { return m_Lat_d; }
        }

        public new double Alt_m
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
                                            1234,
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

        public Suite FindSuiteByName(string suiteName)
        {
            if (Suites.ContainsKey(suiteName))
                return Suites[suiteName];

            return null;
        }
    }
}

