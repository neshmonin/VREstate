using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CoreClasses
{
    public class SuiteClass
    {
        private Vre.Server.BusinessLogic.SuiteType _suiteType;

        public const string SUITESWEBDIR = "SuitesWeb";
        static private Dictionary<string, SuiteClass> m_suiteClasses = new Dictionary<string, SuiteClass>();
        static public Dictionary<string, SuiteClass> SuiteClasses
        {
            get { return m_suiteClasses; }
            set { m_suiteClasses = value; }
        }

        protected SuiteClass() { }

        static public SuiteClass Create(Vre.Server.BusinessLogic.SuiteType suiteType)
        {
            SuiteClass suiteClass = new SuiteClass();
            suiteClass._suiteType = suiteType;
            suiteClass.classId = suiteType.Name;
            SuiteClass val = null;
            if (!m_suiteClasses.TryGetValue(suiteType.Name, out val))
            {
                m_suiteClasses.Add(suiteType.Name, suiteClass);
            }
            //suiteClass.m_geometries.Add(new Geometry(geomelt,
            //                                        suite.Lat_d,
            //                                        suite.Lon_d,
            //                                        suite.Alt_m));

            return suiteClass;
        }

        public string classId;
        private List<Geometry> m_geometries = new List<Geometry>();

        public List<Geometry> Geometries
        {
            get { return m_geometries; }
            set { m_geometries = value; }
        }

        public List<InnerLevel> Floors = new List<InnerLevel>();
        public Uri htmlPageUri = null;

        public string Bedrooms
        {
            get
            {
                string den = _suiteType.DenCount == 0 ? "" : " + D"; 
                return _suiteType.BedroomCount + "Br" + den;
            }
        }
        public string Bathrooms { get { return _suiteType.Bathrooms.ToString(); } }
        public string Balcony { get { return _suiteType.BalconyCount == 0 ? "none" : "yes"; } }
        public string Terrace { get { return _suiteType.TerraceCount == 0 ? "none" : "yes"; } }
        public string Area { get { return _suiteType.FloorAreaString; } }
    }
}
