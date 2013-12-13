using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Vre.Server.Model.Kmz
{
    public class Building : ArchUnit
    {
        public const string XmlPrefix = "_building_";
		protected override string UnitTypeName { get { return "Building"; } }

		public IEnumerable<Suite> Suites { get { return _suites; } }

		private List<Suite> _suites;

		public Building(ConstructionSite parent, string id, string unitName, string unitType, XmlNode unitModel,
			Dictionary<string, XmlNode> models, TMatrix tMatrix)
			: base(parent, id, unitName, unitType, unitModel, models, tMatrix) { }

        protected override void init(XmlNode unitModel, Dictionary<string, XmlNode> models, StringBuilder fatalErrors)
		{
			_suites = new List<Suite>();

            foreach (XmlNode n in unitModel.ChildNodes)
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
                    catch (FormatException)
                    {
                        fatalErrors.AppendFormat("\r\nSuite's node name attribute is invalid: '{0}'", na.Value);
                    }
                }
            }
        }
    }
}