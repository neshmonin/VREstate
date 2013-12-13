using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Vre.Server.Model.Kmz
{
    public abstract class ArchUnit
    {
		protected abstract string UnitTypeName { get; }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        /// <summary>
        /// Currently this is calculated IMPROPERLY
        /// </summary>
        public EcefViewPoint LocationCart { get; private set; }
        /// <summary>
        /// Calculated in alternative way compared to <see cref="LocationCart"/>; tested to be correct.
        /// </summary>
        public ViewPoint LocationGeo { get; private set; }
        public double UnitInMeters { get; private set; }

        protected TMatrix _transformation;
        internal ConstructionSite _site;

		protected virtual void init(XmlNode unitModel, Dictionary<string, XmlNode> models, StringBuilder errors) { }

        public ArchUnit(ConstructionSite parent, string id, string unitName, string unitType, XmlNode unitModel, 
            Dictionary<string, XmlNode> models, TMatrix tMatrix)
        {
            StringBuilder fatalErrors = new StringBuilder();
            Id = id;
            UnitInMeters = parent.UnitInMeters;
            Name = unitName;
            Type = unitType;
            LocationCart = tMatrix.Transform(parent.LocationCart);
            LocationGeo = tMatrix.Point3D2ViewPoint(tMatrix.Transform(new Geometry.Point3D(0.0, 0.0, 0.0)), parent.LocationGeo);
            LocationGeo.Heading += tMatrix.Heading_d_patch; if (LocationGeo.Heading >= 360.0) LocationGeo.Heading -= 360.0;

            _site = parent;
            _transformation = tMatrix;

			init(unitModel, models, fatalErrors);

            if (fatalErrors.Length > 0)
                throw new InvalidDataException(string.Format(
                    "{0} \'{1}\'({2}) - detected problems: {3}",
					UnitTypeName, unitName, unitType, fatalErrors.ToString()));
        }
    }
}