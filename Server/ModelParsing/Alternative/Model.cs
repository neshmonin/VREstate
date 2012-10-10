using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Vre.Server.Model.Kmz
{
    public class Model
    {
        public enum MeasureScale { Meters, Inches };
        
        public string Id { get; private set; }
        public string Name { get; private set; }
        public ViewPoint Location { get; private set; }
        public Scale Scale { get; private set; }
        public MeasureScale UnitScale { get; private set; }
        public double UnitInMeters { get; private set; }
        public ConstructionSite Site { get; private set; }

        public string ColladaModelVersion { get; private set; }
        public string AltitudeMode { get; private set; }

        public Model(Kmz store, XmlNode root, StringBuilder readWarnings)
        {
            const string IdAttributeName = "id";
            const string AltitudeModeNodeName = "altitudeMode";
            const string LocationNodeName = "Location";
            const string OrientationNodeName = "Orientation";
            const string ScaleNodeName = "Scale";
            const string LinkNodeName = "Link";

            Name = store.Name;

            XmlAttribute xattr = root.Attributes[IdAttributeName];
            if (xattr != null) Id = xattr.Value;

            GeoPoint loc = null;
            XmlNode ornode = null;
            XmlDocument core = null;

            XmlNode node = root.FirstChild;
            while (node != null)
            {
                if (node.Name.Equals(AltitudeModeNodeName))
                {
                    AltitudeMode = node.InnerText;
                }
                else if (node.Name.Equals(LocationNodeName))
                {
                    loc = new GeoPoint(node);
                    if (ornode != null) Location = new ViewPoint(loc, ornode);
                }
                else if (node.Name.Equals(OrientationNodeName))
                {
                    if (loc != null) Location = new ViewPoint(loc, node);
                    else ornode = node;
                }
                else if (node.Name.Equals(ScaleNodeName))
                {
                    Scale = new Scale(node);
                }
                else if (node.Name.Equals(LinkNodeName))
                {
                    core = readModelCoreFile(store, node, readWarnings);
                }

                node = node.NextSibling;
            }

            if (null == core) throw new InvalidDataException("Model file does not contain a link to core model file");
            if (null == Location) throw new InvalidDataException("Model file does not contain location information");

            readDocumentRoot(core, readWarnings);
        }

        private XmlDocument readModelCoreFile(Kmz store, XmlNode root, StringBuilder readWarnings)
        {
            const string LinkFileNodeName = "href";

            string filePath = null;

            XmlNode node = root.FirstChild;
            while (node != null)
            {
                if (node.Name.Equals(LinkFileNodeName)) filePath = node.InnerText;

                node = node.NextSibling;
            }

            if (filePath != null)
            {
                Stream s = store.GetFile(filePath);
                if (null == s) throw new InvalidDataException("Model file does not contain core model file: " + filePath);

                XmlDocument result = new XmlDocument();
                result.Load(s);
                return result;
            }
            else
            {
                throw new InvalidDataException("Link node does not contain reference to core model file");
            }
        }

        private void readDocumentRoot(XmlDocument doc, StringBuilder readWarnings)
        {
            const string RootNodeName = "COLLADA";

            XmlNode node = doc.FirstChild;

            while (node != null)
            {
                if (node.Name.Equals(RootNodeName))
                {
                    XmlAttribute ver = node.Attributes["version"];
                    if (ver != null)
                    {
                        ColladaModelVersion = ver.Value;
                        if (!ColladaModelVersion.Equals("1.4.1")) 
                            readWarnings.Append("\r\nCollada model version may be incompatible; expected is 1.4.1; actual is " + ColladaModelVersion);
                    }

                    readColladaStructure(node, readWarnings);
                }
                node = node.NextSibling;
            }
        }

        private void readColladaStructure(XmlNode root, StringBuilder readWarnings)
        {
            const string AssetNodeName = "asset";
            const string ScenesNodeName = "library_visual_scenes";
            const string NodesNodeName = "library_nodes";
            const string GeometriesNodeName = "library_geometries";

            XmlNode node = root[AssetNodeName];
            if (null == node) throw new InvalidDataException("asset node does not exist in Collada structure");

            {
                node = node["unit"];
                if (null == node) throw new InvalidDataException("unit node does not exist in Collada structure");

                XmlAttribute na = node.Attributes["meter"];
                UnitInMeters = double.Parse(na.Value);
                if (Math.Abs(UnitInMeters - 0.025) < 0.001) UnitScale = MeasureScale.Inches;
                else if (Math.Abs(UnitInMeters - 1.0) < 0.001) UnitScale = MeasureScale.Meters;
                else throw new InvalidDataException("unknown unit scale");

                // up-axis
                // https://collada.org/public_forum/viewtopic.php?p=4533#p4533
                /*
                See page 58 (<asset> element section 5-18) of the COLLADA 1.5 specification:

                Up Axis Values

                The <up_axis> element’s values have the following meanings: 		
	
                Code:
                Value       Right Axis       Up Axis           In Axis 
                X-UP        Negative y       Positive x        Positive z 
                Y_UP        Positive x       Positive y        Positive z 
                Z_UP        Positive x       Positive z        Negative y

                Where "In axis" is the direction looking into the scene frustum (near clip) and off into the distant (far clip).
                 */
            }

            // scan nodes and build an <id>-<node> xref
            //
            node = root[NodesNodeName];
            if (null == node) throw new InvalidDataException("nodes node does not exist in Collada structure");

            Dictionary<string, XmlNode> models = new Dictionary<string, XmlNode>();

            foreach (XmlNode n in node.ChildNodes)
            {
                if (!n.Name.Equals("node")) continue;

                XmlAttribute na = n.Attributes["id"];
                if (null == na) continue;

                models.Add(na.Value, n);
            }

            // get a geometry node (used by ConstructionSite object)
            //
            XmlNode geometryNode = root[GeometriesNodeName];
            if (null == geometryNode) throw new InvalidDataException("geometry node does not exist in Collada structure");

            // find a construction site node
            //
            node = root[ScenesNodeName];
            if (null == node) throw new InvalidDataException("scene node does not exist in Collada structure");

            node = node["visual_scene"];
            if (null == node) throw new InvalidDataException("scene node does not contain a subnode in Collada structure");

            // Creation of TMatrix object for the Model - so that ConstructionSite could adjust its TMatrix accordingly
            TMatrix tMatrix = new TMatrix(Location.Heading);
            Site = new ConstructionSite(this, Name, node, models, geometryNode, tMatrix);
        }
    }
}