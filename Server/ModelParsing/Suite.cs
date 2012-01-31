using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
//using NUnit.Framework;

namespace VrEstate
{
    public class Suite : Model, CoreClasses.ICountable, CoreClasses.ICSV, IComparable
    {
        public enum SaleStatus
        {
            Available,
            OnHold,
            Sold
        }

        public enum ToStringStyle
        {
            SortByType,
            SortByFloor,
            SortByStatus,
            SortByPrice
        }

        private Dictionary<SaleStatus, string> m_saleStatusIdCollection = new Dictionary<SaleStatus, string>();

        private string m_name;
        private string m_floorNumber;
        private int m_cellingHeight;
        private double m_price;
        private bool m_showPanoramicView = true;

        private static ToStringStyle m_toStringStyle = ToStringStyle.SortByType;

        public string getPriceString()
        {
            decimal money = (decimal)Math.Round(m_price);

            string result = string.Empty;
            int t = (int)money;
            if ((decimal)t == money)
                result = string.Format("{0:#,#}", money);
            else
                result = string.Format("{0:#,#.00}", money);

            return result;
        }

        private string m_classId;

        public string ClassId { get { return m_classId; } }

        private string m_id;
        private double[][]  m_matrix;
        private double[] m_LonLatAlt;
        private List<string> m_geometryIdList;
        public string[] GeometryIdList { get { return m_geometryIdList.ToArray(); } }
        public double LonModel_d { get { return m_LonLatAlt[0]; } }
        public double LatModel_d { get { return m_LonLatAlt[1]; } }
        public double AltModel_d { get { return m_LonLatAlt[2]; } }

        private SaleStatus m_saleStatus = SaleStatus.Available;
        private double m_heading = 0.0f;

        public Suite(XmlElement node, XmlElement library_nodes_Node, double[][] buildingMatrix)
        {
            m_geometryIdList = new List<string>();

            m_id = node.GetAttribute("id");

            string uniqueKey = node.GetAttribute("name");
            //Debug.Print("Suite {0} Creating", uniqueKey);
            // the convention for uniqueKey value: "_<SuiteName>_<FloorNumber>_<Area>"
            string[] name_floor_cellings_price_view_class = uniqueKey.TrimStart('_').Split('_');
            m_name = name_floor_cellings_price_view_class[0];
            m_floorNumber = name_floor_cellings_price_view_class[1];
            m_cellingHeight = int.Parse(name_floor_cellings_price_view_class[2]);

            m_price = 0.0;
            m_showPanoramicView = true;
            try { m_price = double.Parse(name_floor_cellings_price_view_class[3]);
            } catch {}
            try { m_showPanoramicView = !(name_floor_cellings_price_view_class[4] == "0"
                                       || name_floor_cellings_price_view_class[4] == "no"
                                       || name_floor_cellings_price_view_class[4] == "false"
                                       || name_floor_cellings_price_view_class[4] == "No"
                                       || name_floor_cellings_price_view_class[4] == "False"
                                       || name_floor_cellings_price_view_class[4] == "NO"
                                       || name_floor_cellings_price_view_class[4] == "FALSE");
            } catch {}
            m_classId = name_floor_cellings_price_view_class[name_floor_cellings_price_view_class.Length - 1];

            //if (m_name == "0116" && m_classId == "G1G1")
            //{
            //    int f = 0;
            //}
            string mySaleStatusId = string.Empty;
            m_matrix = ExtractMatrix_InstanceNodeURL(node, out mySaleStatusId);


            m_LonLatAlt = XYZ2LonLatAltRelative(m_matrix[0][3], m_matrix[1][3], m_matrix[2][3],
                                                buildingMatrix);

            m_heading = Math.Round(Angle.Heading_dFromMatrix(m_matrix));

            // now we'll populate the mySaleStatusIdCollection for each suite:
            foreach (var childNode in library_nodes_Node.ChildNodes)
            {
                XmlElement elt = childNode as XmlElement;
                if (elt == null)
                    continue;

                if (elt.Name != "node")
                    continue;



                // if we got here, then elt is the node of the Suite type
                string saleStatId = elt.GetAttribute("id");
                string name = elt.GetAttribute("name").TrimStart('_');
                if (saleStatId == mySaleStatusId && name == m_classId)
                {   // if we got here then this is the contour-supported model and we have
                    // only one generic Class with no sales satatus
                    foreach (var subchild in elt.ChildNodes)
                    {
                        XmlElement subelt = subchild as XmlElement;
                        if (subelt != null && subelt.Name == "instance_geometry")
                        {
                            string geometryUrl = subelt.GetAttribute("url");
                            if (geometryUrl.StartsWith("#")) geometryUrl = geometryUrl.Substring(1);
                            m_geometryIdList.Add(geometryUrl);
                        }
                    }
                }
                else // this is an old-style model
                {
                    if (!name.Contains("_" + Suite.SaleStatus.Available.ToString()) &&
                        !name.Contains("_" + Suite.SaleStatus.OnHold.ToString()) &&
                        !name.Contains("_Busy") &&
                        !name.Contains("_" + Suite.SaleStatus.Sold.ToString()))
                        continue;
                }






                //// if we got here, then elt is the node of the Suite type
                //string name = elt.GetAttribute("name").TrimStart('_');
                //if (!name.Contains("_" + Suite.SaleStatus.Available.ToString()) &&
                //    !name.Contains("_" + Suite.SaleStatus.OnHold.ToString()) &&
                //    !name.Contains("_Busy") &&
                //    !name.Contains("_" + Suite.SaleStatus.Sold.ToString()))
                //    continue;

                string eltName = string.Format("{0}_", ClassId);
                if (name.StartsWith(eltName))
                {
                    //string saleStatId = elt.GetAttribute("id");
                    if (name.Contains(Suite.SaleStatus.Available.ToString()))
                    {
                        m_saleStatusIdCollection.Add(Suite.SaleStatus.Available, saleStatId);
                        if (mySaleStatusId == saleStatId)
                            m_saleStatus = SaleStatus.Available;
                    }
                    else
                    if (name.Contains(Suite.SaleStatus.OnHold.ToString()) || name.Contains("Busy"))
                    {
                        m_saleStatusIdCollection.Add(Suite.SaleStatus.OnHold, saleStatId);
                        if (mySaleStatusId == saleStatId)
                        {
                            m_saleStatus = SaleStatus.OnHold;
                        }
                    }
                    else
                    if (name.Contains(Suite.SaleStatus.Sold.ToString()))
                    {
                        m_saleStatusIdCollection.Add(Suite.SaleStatus.Sold, saleStatId);
                        if (mySaleStatusId == saleStatId)
                            m_saleStatus = SaleStatus.Sold;
                    }
                }
            }

            //if (m_floorNumber == 66 || m_floorNumber == 67)
            //{
            //    string val1, val2, val3;
            //    Debug.Print("Suite {0}:\t{1}\t{2}\t{3}", 
            //                uniqueKey,
            //                m_saleStatusIdCollection.TryGetValue(SaleStatus.Available, out val1) ? val1 : "NO VALUE",
            //                m_saleStatusIdCollection.TryGetValue(SaleStatus.OnHold, out val2) ? val2 : "NO VALUE",
            //                m_saleStatusIdCollection.TryGetValue(SaleStatus.Sold, out val3) ? val3 : "NO VALUE");
            //}
        }

        public string ToCSV()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6}\n",
                                 FloorNumber,
                                 Name,
                                 ClassId,
                                 CellingHeight,
                                 Price,
                                 Status,
                                 ShowPanoramicView?"Show":"Hide");
        }

        public bool FromCSV(string csv)
        {
            string[] split = csv.Split(new char[] { ',', ';', '\t' });
            if (split.Length != 7) return false;

            string floor = split[0].Trim();
            int floorNum = -1;
            try { floorNum = int.Parse(floor); } catch (System.FormatException) { }
            if (floorNum != -1)
                floor = string.Format("{0:D2}", floorNum);

            if (floor != FloorNumber) return false;

            string name = split[1].Trim();
            int suiteNum = -1;
            try { suiteNum = int.Parse(name); } catch (System.FormatException) { }
            if (suiteNum != -1)
                name = string.Format("{0:D4}", suiteNum);

            if (name != Name) return false;

            string classID = split[2].Trim();
            if (classID != ClassId) return false;

            int cellingHeight = 0;
            try { cellingHeight = int.Parse(split[3].Trim()); }
            catch { return false; }

            double price = 0.0;
            try { price = double.Parse(split[4].Trim()); }
            catch { return false; }

            SaleStatus status = SaleStatus.Available;
            try { status = (SaleStatus)SaleStatus.Parse(typeof(SaleStatus), split[5].Trim(), true); }
            catch { return false; }

            string showPanoramicView = split[6].Trim().ToLower();
            switch (showPanoramicView)
            {
                case "show":
                    ShowPanoramicView = true;
                    break;
                case "hide":
                    ShowPanoramicView = false;
                    break;
                default:
                    return false;
            }

            FloorNumber = floor;
            Name = name;
            m_classId = classID;
            CellingHeight = cellingHeight;
            Price = price;
            Status = status;

            return true;
        }

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public string FloorNumber
        {
            get { return m_floorNumber; }
            set { m_floorNumber = value; }
        }

        public string Id {get { return m_id; }}

        public string SaleStatusId
        {
            get { return m_saleStatusIdCollection[Status]; }
        }

        public double Lon_d { get { return m_LonLatAlt[0]; } }
        public double Lat_d { get { return m_LonLatAlt[1]; } }
        public double Alt_i
        {
            get
            {
                if (Site.Scale == Site.MeasureScale.Inches)
                    return m_LonLatAlt[2];

                return m_LonLatAlt[2] / Site.i2m;
            }
        }
        public double Alt_m
        {
            get
            {
                if (Site.Scale == Site.MeasureScale.Meters)
                    return m_LonLatAlt[2];

                return m_LonLatAlt[2] * Site.i2m;
            }
        }

        public int HowManyItems { get { return 1; } }

        public double Heading_d
        {
            get { return m_heading; }
        }

        public SaleStatus Status
        {
            get { return m_saleStatus; }
            set { m_saleStatus = value; }
        }

        public double Price
        {
            get { return m_price; }
            set { m_price = value; }
        }

        public int CellingHeight
        {
            get { return m_cellingHeight; }
            set { m_cellingHeight = value; }
        }

        public List<SaleStatus> SaleStatuses
        {
            get
            {
                List<SaleStatus> statList = m_saleStatusIdCollection.Keys.ToList<SaleStatus>();
                statList.Sort();
                return statList;
            }
        }

        public bool ShowPanoramicView
        {
            get { return m_showPanoramicView; }
            set { m_showPanoramicView = value; }
        }

        public static ToStringStyle ToStringSort
        {
            get { return m_toStringStyle; }
            set { m_toStringStyle = value; }
        }

        public string UniqueKey
        {
            get
            {
                string uniqueKey = string.Format("_{0}_{1}_{2}_{3}_{4}_{5}",
                                                 m_name,
                                                 m_floorNumber,
                                                 m_cellingHeight,
                                                 m_price,
                                                 m_showPanoramicView?"yes":"no",
                                                 m_classId);
                return uniqueKey;
            }
        }

        public override string ToString()
        {
            switch (m_toStringStyle)
            {
                case ToStringStyle.SortByType:
                    return string.Format("{0}\t{2}\t{1}\t{3}\t{4}\t{5}\t{6}",
                                        ClassId, m_name, m_floorNumber,
                                        m_cellingHeight, m_price, Status,
                                        m_showPanoramicView?"Show":"Hide");
                case ToStringStyle.SortByFloor:
                    return string.Format("{2}\t{1}\t{0}\t{3}\t{4}\t{5}\t{6}",
                                        ClassId, m_name, m_floorNumber,
                                        m_cellingHeight, m_price, Status,
                                        m_showPanoramicView ? "Show" : "Hide");
                case ToStringStyle.SortByStatus:
                    return string.Format("{5}\t{0}\t{1}\t{2}\t{3}\t{4}\t{6}",
                                        ClassId, m_name, m_floorNumber,
                                        m_cellingHeight, m_price, Status,
                                        m_showPanoramicView ? "Show" : "Hide");
                case ToStringStyle.SortByPrice:
                    return string.Format("{4}\t{2}\t{0}\t{1}\t{3}\t{4}\t{6}",
                                        ClassId, m_name, m_floorNumber,
                                        m_cellingHeight, m_price, Status,
                                        m_showPanoramicView ? "Show" : "Hide");
                default:
                    return UniqueKey;
            }
        }

        public void Save(ref XmlElement node)
        {
            node.SetAttribute("name", UniqueKey);

            foreach (var childNode in node.ChildNodes)
            {
                XmlElement elt = childNode as XmlElement;
                if (elt.Name == "instance_node")
                {
                    string toSave = "#" + SaleStatusId;
                    elt.SetAttribute("url", toSave);
                    break;
                }
            }
        }

        // Returns:
        //     A value that indicates the relative order of the objects being compared.
        //     The return value has these meanings: Value Meaning Less than zero This instance
        //     is less than obj. Zero This instance is equal to obj. Greater than zero This
        //     instance is greater than obj.
        public int CompareTo(object obj)
        {
            Suite that = obj as Suite;
            string meStr = ToString().ToLower();
            string thatStr = that.ToString().ToLower();
            if (meStr.StartsWith("ph") && thatStr.StartsWith("rg"))
                return 1;
            if (meStr.StartsWith("rg") && thatStr.StartsWith("ph"))
                return -1;

            return meStr.CompareTo(thatStr);
        }
    }
}
