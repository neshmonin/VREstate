using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
//using NUnit.Framework;

namespace CoreClasses
{
    public class Suite : Model, ICountable, ICSV, IComparable
    {
        private Vre.Server.BusinessLogic.Client.SuiteEx _suite;

        public Vre.Server.BusinessLogic.ClientData ClientData
        {
            get { return _suite.GetClientData(); }
        }

        public enum SaleStatus
        {
            Available,
            OnHold,
            Sold,
            None
        }

        private List<SaleStatus> saleStatuses = new List<SaleStatus>(3);

        public List<SaleStatus> SaleStatuses { get { return saleStatuses; } }

        public string getPriceString()
        {
            decimal money = (decimal)Math.Round(_suite.CurrentPrice);

            string result = string.Empty;
            int t = (int)money;
            if ((decimal)t == money)
                result = string.Format("{0:#,#}", money);
            else
                result = string.Format("{0:#,#.00}", money);

            if (string.IsNullOrEmpty(result))
                result = "N/A";

            return result;
        }

        public string ClassId
        {
            get
            {
                return _suite.SuiteType.Name;
            }
        }

        private double m_heading = 0.0f;
        //int stop = 0;
        public Suite(Vre.Server.BusinessLogic.Client.SuiteEx suite)
        {
            _suite = suite;
            saleStatuses.Add(SaleStatus.Available);
            saleStatuses.Add(SaleStatus.OnHold);
            saleStatuses.Add(SaleStatus.Sold);

            m_heading = _suite.Location.HorizontalHeading;
        }

        public Suite MakeClone()
        {
            Vre.Server.BusinessLogic.Client.SuiteEx _clone = 
                      new Vre.Server.BusinessLogic.Client.SuiteEx(_suite, _suite.CurrentPrice);
            Suite clone = new Suite(_clone);

            return clone;
        }

        public SuiteClass SuiteClass
        {
            get
            {
                SuiteClass suiteClass = null;
                if (SuiteClass.SuiteClasses.TryGetValue(ClassId, out suiteClass))
                    return suiteClass;

                return null;
            }
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

        public virtual bool FromCSV(string csv)
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
            //if (suiteNum != -1)
            //    name = string.Format("{0:D4}", suiteNum);

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
                case "yes":
                    ShowPanoramicView = true;
                    break;
                case "hide":
                case "no":
                    ShowPanoramicView = false;
                    break;
                default:
                    return false;
            }

            FloorNumber = floor;
            Name = name;
            _suite.SuiteType.Name = classID;
            CellingHeight = cellingHeight;
            Price = price;
            Status = status;

            return true;
        }

        public string Name
        {
            get { return _suite.SuiteName; }
            set { _suite.SuiteName = value; }
        }

        public string FloorNumber
        {
            get { return _suite.FloorName; }
            set { _suite.FloorName = value; }
        }

        public new double Lon_d { get { return _suite.Location.Longitude; } }
        public new double Lat_d { get { return _suite.Location.Latitude; } }
        public double Alt_i
        {
            get
            {
                if (Site.Scale == Site.MeasureScale.Inches)
                    return _suite.Location.Altitude;

                return _suite.Location.Altitude / Site.i2m;
            }
        }
        public new double Alt_m
        {
            get
            {
                if (Site.Scale == Site.MeasureScale.Meters)
                    return _suite.Location.Altitude;

                return _suite.Location.Altitude * Site.i2m;
            }
        }

        public int HowManyItems { get { return 1; } }

        public double Heading_d
        {
            get { return m_heading; }
        }

        public SaleStatus Status
        {
            get
            {
                switch (_suite.Status)
                {
                    case Vre.Server.BusinessLogic.Suite.SalesStatus.Available:
                        return SaleStatus.Available;
                    case Vre.Server.BusinessLogic.Suite.SalesStatus.OnHold:
                        return SaleStatus.OnHold;
                    case Vre.Server.BusinessLogic.Suite.SalesStatus.Sold:
                        return SaleStatus.Sold;
                    default:
                        return SaleStatus.None;
                }
            }
            set
            {
                switch (value)
                {
                    case SaleStatus.Available:
                        _suite.Status = Vre.Server.BusinessLogic.Suite.SalesStatus.Available;
                        return;
                    case SaleStatus.OnHold:
                        _suite.Status = Vre.Server.BusinessLogic.Suite.SalesStatus.OnHold;
                        return;
                    case SaleStatus.Sold:
                        _suite.Status = Vre.Server.BusinessLogic.Suite.SalesStatus.Sold;
                        return;
                }
            }
        }

        public double Price
        {
            get { return _suite.CurrentPrice; }
            set { _suite.CurrentPrice = value; }
        }

        public int CellingHeight
        {
            get
            {
                double feet = _suite.CeilingHeight.ValueAs(Vre.Server.BusinessLogic.ValueWithUM.Unit.Feet);
                return (int)Math.Round(feet);
            }
            set
            {
                _suite.CeilingHeight.SetValue((double)value, Vre.Server.BusinessLogic.ValueWithUM.Unit.Feet);
            }
        }

        public bool ShowPanoramicView
        {
            get { return _suite.ShowPanoramicView; }
            set { _suite.ShowPanoramicView = value; }
        }

        public string UniqueKey
        {
            get
            {
                string uniqueKey = string.Format("_{0}_{1}", Name, ClassId);
                return uniqueKey;
            }
        }

        public static string UniqueKeyFromCSVline(string csvLine)
        {
            string[] split = csvLine.Split(new char[] { ',', ';', '\t' });
            if (split.Length != 7) return "";
            string uniqueKey = string.Format("_{0}_{1}", split[1], split[2]);
            return uniqueKey;
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}",
                                _suite.FloorName, Name, ClassId, 
                                CellingHeight, _suite.CurrentPrice, Status,
                                _suite.ShowPanoramicView ? "Show" : "Hide");
        }

        public virtual string[] ToStringArray()
        {
            int sepIndx = ClassId.LastIndexOf('/');
            string SuiteTypeOnly = ClassId.Substring(sepIndx + 1);
            string[] retArray = new string[6]
                                    {
                                        Name,
                                        SuiteTypeOnly,
                                        CellingHeight.ToString(),
                                        String.Format("{0:N2}", _suite.CurrentPrice),
                                        Status.ToString(),
                                        _suite.ShowPanoramicView ? "Show" : "Hide"
                                    };

            return retArray;                                    
        }

        // Returns:
        //     A value that indicates the relative order of the objects being compared.
        //     The return value has the following meanings:
        //  <Less than zero>:    This instance is less than the obj.
        //  <Zero>:              This instance is equal to the obj.
        //  <Greater than zero>: This instance is greater than the obj.
        public int CompareTo(object obj)
        {
            Suite that = obj as Suite;
            string meStr = ToString().ToLower().TrimStart(' ','+','>');
            string thatStr = that.ToString().ToLower().TrimStart(' ','+','>');
            if (meStr.StartsWith("gf")) meStr.Replace("gf","01");
            if (thatStr.StartsWith("gf")) thatStr.Replace("gf", "01"); ;

            if (meStr.StartsWith("ph") && thatStr.StartsWith("rg"))
                return 1;
            if (meStr.StartsWith("rg") && thatStr.StartsWith("ph"))
                return -1;

            return meStr.CompareTo(thatStr);
        }

        public bool Equals(Suite that)
        {
            return Name == that.Name &&
                   FloorNumber == that.FloorNumber &&
                   ClassId == that.ClassId;
        }

        public override bool Equals(Object obj)
        {
            Suite thatSuite = obj as Suite;
            if (thatSuite != null)
                return this.Equals(thatSuite);

            string thatStr = obj.ToString();
            if (thatStr != null)
                return ToString().Equals(thatStr);

            return false;
        }
    }
}
