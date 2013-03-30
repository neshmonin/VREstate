using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;

namespace CoreClasses
{
    public class Site : CoreClasses.ICountable
    {
        protected Vre.Server.BusinessLogic.Site _site;

        public const double i2m = 0.0254;

        private Dictionary<string, Building> m_buildings = new Dictionary<string, Building>();
        private double m_unitInMeters;
        public enum MeasureScale { Meters, Inches };
        public static MeasureScale m_scale = MeasureScale.Inches;

        private double m_Lon_d = 0;
        private double m_Lat_d = 0;
        private double m_Alt_m = 0;
        public static double LonModel_d { get { return Model.Lon_d; } }
        public static double LatModel_d { get { return Model.Lat_d; } }

        public Site(Vre.Server.BusinessLogic.Site site) { _site = site; }

        public Site Create(Vre.Server.BusinessLogic.Site site)
        {
            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                              "suitetype",
                                                              "site=" + site.AutoID,
                                                              null);
            if (HttpStatusCode.OK == resp.ResponseCode)
            {
                Vre.Server.BusinessLogic.ClientData suiteTypeist = resp.Data;

                foreach (Vre.Server.BusinessLogic.ClientData cd in suiteTypeist.GetNextLevelDataArray("suiteTypes"))
                {
                    SuiteClass suiteClass = SuiteClass.Create(new Vre.Server.BusinessLogic.SuiteType(cd, site));
                }
            }

            resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                              "building",
                                                              "site=" + site.AutoID,
                                                              null);
            if (HttpStatusCode.OK != resp.ResponseCode)
                return null;

            Vre.Server.BusinessLogic.ClientData buildingList = resp.Data;

            foreach (Vre.Server.BusinessLogic.ClientData cd in buildingList.GetNextLevelDataArray("buildings"))
            {
                Building building = CreateBuilding(cd);
                if (building != null)
                {
                    m_Lon_d += building.Lon_d;
                    m_Lat_d += building.Lat_d;
                    m_Alt_m += building.Alt_m;

                    m_buildings.Add(building.Name, building);
                }
            }

            // average up the Lon-s/Lat-s/Alt-s - to obtain the coordinates of the building center
            m_Lon_d = m_Lon_d / m_buildings.Count;
            m_Lat_d = m_Lat_d / m_buildings.Count;
            m_Alt_m = m_Alt_m / m_buildings.Count;

            return this;
        }

        protected virtual Building CreateBuilding(Vre.Server.BusinessLogic.ClientData cd)
        {
            Vre.Server.BusinessLogic.Building vre_building = new Vre.Server.BusinessLogic.Building(cd, _site);
            Building newBuilding = new Building(vre_building);
            return newBuilding.Create(vre_building);
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

        public Dictionary<string, Building> Buildings
        {
            get { return m_buildings; }
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

        public string Name { get { return _site.Name; } }

        public static MeasureScale Scale
        {
            get { return m_scale; }
            set { m_scale = value; }
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
