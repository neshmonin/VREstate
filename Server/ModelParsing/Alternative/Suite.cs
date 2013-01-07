using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Vre.Server.Model.Kmz
{
    public class Suite
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Floor { get; private set; }
        public int CeilingHeightFt { get; private set; }
        public bool ShowPanoramicView { get; private set; }
        public double InitialPrice { get; private set; }
        /// <summary>
        /// Currently this is calculated IMPROPERLY
        /// </summary>
        public EcefViewPoint LocationCart { get; private set; }
        /// <summary>
        /// Calculated in alternative way compared to <see cref="LocationCart"/>; tested to be correct.
        /// </summary>
        public ViewPoint LocationGeo { get; private set; }

        public string ClassName { get; private set; }
        public IEnumerable<string> GeometryIdList { get { return _geometryIdList; } }
        public TMatrix Matrix { get; private set; }

        private List<string> _geometryIdList;
    
        public Suite(Building parent, string id, string suiteDescription, XmlNode suiteModel,
            Dictionary<string, XmlNode> models, TMatrix tMatrix)
        {
            string suiteDescriptionDelimited = suiteDescription.Replace("__", "_").Trim('_');
            string[] parts = suiteDescriptionDelimited.Split('_');

            Id = id;
            Name = parts[0];
            //LocationCart = tMatrix.Transform(parent.LocationCart);
            LocationCart = tMatrix.Transform(parent._site.LocationCart);
            LocationGeo = tMatrix.Point3D2ViewPoint(tMatrix.Transform(new Geometry.Point3D(0.0, 0.0, 0.0)), parent._site.LocationGeo);
            LocationGeo.Heading += tMatrix.Heading_d_patch; if (LocationGeo.Heading >= 360.0) LocationGeo.Heading -= 360.0;
            Matrix = tMatrix;

            // format of the suite description could be:
            // <name> <floor> <ceiling height> <suite type name>
            // or
            // <name> <floor> <ceiling height> <initial price> <show panoramic view> <suite type name>
            // code-wise a lot of intermediate variants are parseable as well
            Floor = null;
            CeilingHeightFt = 0;
            ShowPanoramicView = true;
            InitialPrice = 0.0;
            if (parts.Length > 1)
            {
                Floor = parts[1];

                if (parts.Length > 3)
                {
                    CeilingHeightFt = int.Parse(parts[2]);

                    if (parts.Length > 4)  // assume last required element is suite type name
                    {
                        try { InitialPrice = double.Parse(parts[3], CultureInfo.InvariantCulture); }
                        catch (FormatException) { }

                        if (parts.Length > 5)  // assume last required element is suite type name
                        {
                            string spw = parts[4].ToLower();
                            ShowPanoramicView = !(spw.Equals("0") || spw.Equals("no") || spw.Equals("false"));
                        }
                    }
                }
            }

            ClassName = parts[parts.Length - 1];
            XmlAttribute na = suiteModel.Attributes["name"];
            if (na != null)
            {
                string realClassName = na.Value.Trim('_');
                if (!realClassName.Equals(ClassName))
                    throw new InvalidDataException(string.Format(
                        "MDSC22: Suite '{0}->{1}': Definition {2} does not match the last section of the name",
                        parent.Name, suiteDescription, realClassName));
            }
            if (!ClassName.Contains("/")) ClassName = parent.Type + "/" + ClassName;  // legacy support

            _geometryIdList = new List<string>();
            foreach (XmlNode n in suiteModel.ChildNodes)
            {
                if (!n.Name.Equals("instance_geometry")) continue;

                na = n.Attributes["url"];
                if (null == na) continue;

                string geometryId = na.Value;
                if (geometryId.StartsWith("#")) geometryId = geometryId.Substring(1);
                _geometryIdList.Add(geometryId);
            }
        }
    }
}