using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Vre.Server.Model.Kmz
{
    public class Structure : ArchUnit
    {
        public const string XmlPrefix = "_structure_";
		protected override string UnitTypeName { get { return "Structure"; } }

		public Structure(ConstructionSite parent, string id, string unitName, string unitType, XmlNode unitModel,
			Dictionary<string, XmlNode> models, TMatrix tMatrix)
			: base(parent, id, unitName, unitType, unitModel, models, tMatrix) { }
    }
}