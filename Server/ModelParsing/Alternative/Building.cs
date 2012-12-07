using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Vre.Server.Model.Kmz
{
    public class Building
    {
        public const string XmlPrefix = "_building_";
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        public IEnumerable<Suite> Suites { get { return _suites; } }
        /// <summary>
        /// Currently this is calculated IMPROPERLY
        /// </summary>
        public EcefViewPoint LocationCart { get; private set; }
        /// <summary>
        /// Calculated in alternative way compared to <see cref="LocationCart"/>; tested to be correct.
        /// </summary>
        public ViewPoint LocationGeo { get; private set; }
        public double UnitInMeters { get; private set; }

        private List<Suite> _suites;
        private TMatrix _transformation;
        internal ConstructionSite _site;

        public Building(ConstructionSite parent, string id, string buildingName, string buildingType, XmlNode buildingModel, 
            Dictionary<string, XmlNode> models, TMatrix tMatrix)
        {
            StringBuilder fatalErrors = new StringBuilder();
            Id = id;
            UnitInMeters = parent.UnitInMeters;
            Name = buildingName;
            Type = buildingType;
            LocationCart = tMatrix.Transform(parent.LocationCart);
            LocationGeo = tMatrix.Point3D2ViewPoint(tMatrix.Transform(new Geometry.Point3D(0.0, 0.0, 0.0)), parent.LocationGeo);
            LocationGeo.Heading += tMatrix.Heading_d_patch; if (LocationGeo.Heading >= 360.0) LocationGeo.Heading -= 360.0;
            _suites = new List<Suite>();

            _site = parent;
            _transformation = tMatrix;

            foreach (XmlNode n in buildingModel.ChildNodes)
            {
                XmlAttribute na, nna;
                XmlNode nn;
                string nodeId = null;

                if (!n.Name.Equals("node")) continue;

                na = n.Attributes["name"];
                if (null == na) continue;

                if (na.Value.StartsWith("instance_")) continue;  // not a suite!

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

                XmlNode suiteNode = null;
                if (nodeId != null) if (!models.TryGetValue(nodeId, out suiteNode)) suiteNode = null;

                nn = n["matrix"];

                if ((suiteNode != null) && (nn != null) && (na != null))
                {
                    TMatrix matrix = new TMatrix(_transformation, nn.InnerText, UnitInMeters);

                    try
                    {
                        _suites.Add(new Suite(this, nodeId, na.Value, suiteNode, models, matrix));
                    }
                    catch (InvalidDataException ide)
                    {
                        fatalErrors.AppendFormat("\r\n{0}", ide.Message);
                    }
                }
            }

            if (fatalErrors.Length > 0)
                throw new InvalidDataException(string.Format(
                    "Building Name \'{0}\'({1}) - detected problems: {2}", 
                    buildingName, buildingType, fatalErrors.ToString()));
        }
    }
}