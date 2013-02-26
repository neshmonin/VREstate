using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Vre.Server.Mls
{
    public class RetsXmlParser : XmlUrlResolver
    {
        private StringBuilder _readWarnings;
        private List<MlsUnit> _units;

        public IEnumerable<MlsUnit> Units { get { return _units; } }

        //public override Uri ResolveUri(Uri baseUri, string relativeUri)
        //{
        //    if (relativeUri.Equals("RETS-20041001.dtd")) throw new XmlException("No resolution");
        //    return base.ResolveUri(baseUri, relativeUri);
        //}

        //public override bool SupportsType(Uri absoluteUri, Type type)
        //{
        //    if (absoluteUri.AbsolutePath.EndsWith("RETS-20041001.dtd")) return true;
        //    return base.SupportsType(absoluteUri, type);
        //}

        //public override System.Net.ICredentials Credentials
        //{
        //    set {  }
        //}

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri == null)
            {
                throw new ArgumentNullException("absoluteUri");
            }

            if (//isDocumentTypeDefinitionFile(absoluteUri) &&
                (ofObjectToReturn == null || ofObjectToReturn == typeof(Stream)))
            {
                var filePath = "Vre.Server.Mls." + absoluteUri.AbsolutePath.Substring(absoluteUri.AbsolutePath.LastIndexOf('/') + 1);// getFileName(absoluteUri);
                var resourceStream = Assembly.
                    GetExecutingAssembly().
                    GetManifestResourceStream(filePath);

                if (resourceStream != null) return resourceStream;

                //if (resourceStream == null)
                //{
                //    throw new FileNotFoundException("Embeded DTD file not found", filePath);
                //}

                //return resourceStream;
            }

            return base.GetEntity(absoluteUri, role, ofObjectToReturn);
        }

        public string Parse(string fileName)
        {
            _readWarnings = new StringBuilder();
            _units = new List<MlsUnit>();

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.XmlResolver = this;
                using (FileStream file = File.OpenRead(fileName))
                    doc.Load(file);

                XmlNode root = doc.FirstChild;
                while (root != null)
                {
                    if ((root.NodeType == XmlNodeType.Element) && root.Name.Equals("RETS")) readRets(root);
                    root = root.NextSibling;
                }
            }
            catch (Exception e)
            {
                _readWarnings.AppendFormat("Fatal error: {0}\r\n{1}\r\n", e.Message, e.StackTrace);
            }

            string result = _readWarnings.ToString();
            _readWarnings.Length = 0;
            return result;
        }

        private void readRets(XmlNode root)
        {
            XmlNode node = root.FirstChild;
            while (node != null)
            {
                if (node.Name.Equals("REData")) readReData(node);
                node = node.NextSibling;
            }
        }

        private void readReData(XmlNode root)
        {
            XmlNode node = root.FirstChild;
            while (node != null)
            {
                if (node.Name.Equals("REProperties")) readReProps(node);
                node = node.NextSibling;
            }
        }

        private void readReProps(XmlNode root)
        {
            XmlNode node = root.FirstChild;
            while (node != null)
            {
                if (node.Name.Equals("CondoProperty")) readCondoProps(node);
                node = node.NextSibling;
            }
        }

        private void readCondoProps(XmlNode root)
        {
            XmlNode node = root.FirstChild;
            while (node != null)
            {
                if (node.Name.Equals("Listing")) break;
                node = node.NextSibling;
            }
            if (null == node)
            {
                _readWarnings.AppendFormat("Node does not have a Listing node\r\n{0}\r\n", root.OuterXml);
            }
            else
            {
                string mls = null;
                string aptUnit = null;
                string address = null;
                string municipality = null;
                string asf = null;
                string bedrooms = null;
                string bedroomsPlus = null;
                string rooms = null;
                string roomsPlus = null;
                string washrooms = null;
                string balcony = null;
                string listPrice = null;
                string vTourUrl = null;
                string saleLease = null;
                string status = null;

                node = node.FirstChild;
                while (node != null)
                {
                    if (node.Name.Equals("MLS")) mls = node.InnerText;
                    else if (node.Name.Equals("AptUnit")) aptUnit = node.InnerText;
                    else if (node.Name.Equals("Address")) address = node.InnerText;
                    else if (node.Name.Equals("Municipality")) municipality = node.InnerText;
                    else if (node.Name.Equals("ApproxSquareFootage")) asf = node.InnerText;
                    else if (node.Name.Equals("Bedrooms")) bedrooms = node.InnerText;
                    else if (node.Name.Equals("BedroomsPlus")) bedroomsPlus = node.InnerText;
                    else if (node.Name.Equals("Rooms")) rooms = node.InnerText;
                    else if (node.Name.Equals("RoomsPlus")) roomsPlus = node.InnerText;
                    else if (node.Name.Equals("Washrooms")) washrooms = node.InnerText;
                    else if (node.Name.Equals("Balcony")) balcony = node.InnerText;
                    else if (node.Name.Equals("ListPrice")) listPrice = node.InnerText;
                    else if (node.Name.Equals("VirtualTourURL")) vTourUrl = node.InnerText;
                    else if (node.Name.Equals("SaleLease")) saleLease = node.InnerText;
                    else if (node.Name.Equals("Status")) status = node.InnerText;

                    node = node.NextSibling;
                }

                int aasf = 0;
                int bdr = 0;
                int otr = 0;
                int bathr = 0;
                int balc = 0;
                int terr = 0;
                double prc = 0.0;
                MlsUnit.SaleLease sl = MlsUnit.SaleLease.Unknown;

                if ((aptUnit != null) && aptUnit.Equals("null")) aptUnit = null;

                if (asf != null)
                {
                    string[] parts = asf.Split('-');
                    if (2 == parts.Length)
                    {
                        int min, max;
                        if (int.TryParse(parts[0].Trim(), out min) && int.TryParse(parts[1].Trim(), out max))
                            aasf = min + ((max - min) / 2);
                    }
                }

                if (bedrooms != null)
                {
                    int val;
                    if (int.TryParse(bedrooms.Trim(), out val)) bdr = val;
                }
                if (bedroomsPlus != null)
                {
                    int val;
                    if (int.TryParse(bedroomsPlus.Trim(), out val)) bdr += val;
                }

                if (rooms != null)
                {
                    int val;
                    if (int.TryParse(rooms.Trim(), out val)) otr = val;
                }
                if (roomsPlus != null)
                {
                    int val;
                    if (int.TryParse(roomsPlus.Trim(), out val)) otr += val;
                }

                if (washrooms != null)
                {
                    int val;
                    if (int.TryParse(washrooms.Trim(), out val)) bathr = val;
                }

                if (balcony != null)
                {
                    if (balcony.Equals("null")) { balc = 0; terr = 0; }
                    else if (balcony.Equals("None")) { balc = 0; terr = 0; }
                    else if (balcony.Equals("Open")) { balc = 1; terr = 0; }
                    else if (balcony.Equals("Terr")) { balc = 0; terr = 1; }
                    else if (balcony.Equals("Jlte")) { balc = 0; terr = 0; }
                    else if (balcony.Equals("Encl")) { balc = 0; terr = 0; }
                    else _readWarnings.AppendFormat("MLS {0}: Unknown balcony type: {1}\r\n", mls, balcony);
                }

                if (listPrice != null)
                {
                    double val;
                    if (double.TryParse(listPrice.Trim(), out val)) prc = val;
                }

                if (saleLease != null)
                {
                    if (saleLease.Equals("Sale")) sl = MlsUnit.SaleLease.Sale;
                    else if (saleLease.Equals("Lease")) sl = MlsUnit.SaleLease.Lease;
                    else _readWarnings.AppendFormat("MLS {0}: Unknown SaleLease type: {1}\r\n", mls, saleLease);
                }

                if (status != null)
                {
                    if (!status.Equals("A"))
                        _readWarnings.AppendFormat("MLS {0}: Unknown status: {1}\r\n", mls, status);
                }

                mls.Equals(aptUnit);
            }
        }
    }
}
