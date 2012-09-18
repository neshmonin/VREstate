using System.Xml;
using System.Collections.Generic;
using System;

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
        public EcefViewPoint LocationCart { get; private set; }

        public string ClassName { get; private set; }
        public IEnumerable<string> GeometryIdList { get { return _geometryIdList; } }

        private List<string> _geometryIdList;
#if DEBUG
        private TMatrix _transformation;
#endif
    
        public Suite(Building parent, string id, string suiteDescription, XmlNode suiteModel,
            Dictionary<string, XmlNode> models, TMatrix tMatrix)
        {
            string[] parts = suiteDescription.Trim().Split(' ');

            Id = id;
            Name = parts[0];
            LocationCart = tMatrix.Transform(parent.LocationCart);
#if DEBUG
            LocationCart = tMatrix.Transform(parent._site.LocationCart);
            _transformation = tMatrix;
#endif

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
                    try { CeilingHeightFt = int.Parse(parts[2]); }
                    catch (FormatException) { System.Diagnostics.Debugger.Break(); }

                    if (parts.Length > 4)  // assume last required element is suite type name
                    {
                        try { InitialPrice = double.Parse(parts[3]); }
                        catch (FormatException) { }

                        if (parts.Length > 5)  // assume last required element is suite type name
                        {
                            string spw = parts[4].ToLower();
                            ShowPanoramicView = !(spw.Equals("0") || spw.Equals("no") || spw.Equals("false"));
                        }
                    }
                }
            }

            ClassName = null;
            XmlAttribute na = suiteModel.Attributes["name"];
            if (na != null) ClassName = na.Value.Trim('_');

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