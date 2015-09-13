using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CoreClasses;

namespace ConsoleSales
{
    public class ChangingSuite : Suite
    {
        static private Dictionary<string, ChangingSuite> m_changedSuites = new Dictionary<string, ChangingSuite>();

        protected Suite _changed;
        private bool _promoted = false;

        public ChangingSuite(Vre.Server.BusinessLogic.Suite suite)
            : base(suite)
        {
            _changed = base.MakeClone();
        }

        public Suite suite
        {
            get { return _changed; }
        }

        public override bool FromCSV(string csv)
        {
            if (!suite.FromCSV(csv)) return false;

            if (changed)
                ChangingSuite.checkIn(this);

            return true;
        }

        static public bool AtLeastOneChanged
        {
            get
            {
                foreach (var c in m_changedSuites.Values)
                {
                    if (c.changed)
                        return true;
                }

                return false;
            }
        }

        static public int HowManyChanges
        {
            get
            {
                int count = 0;
                foreach (var c in m_changedSuites.Values)
                {
                    if (c.changed)
                        count++;
                }

                return count;
            }
        }

        const int MaxNumberOfRecordsInMessage = 200;

        static public Vre.Server.BusinessLogic.ClientData
            GenerateClientData(out List<ChangingSuite> unitsToUpdate, out int buildingID)
        {
            buildingID = -1;
            Vre.Server.BusinessLogic.Building building = null;
            unitsToUpdate = new List<ChangingSuite>();
            List<Vre.Server.BusinessLogic.ClientData> units = new List<Vre.Server.BusinessLogic.ClientData>();
            foreach (var suite in m_changedSuites.Values)
            {
                if (suite.changed)
                {
                    if (building == null)
                    {
                        building = suite.getBuilding();
                        buildingID = building.AutoID;
                    }

                    if (suite.getBuilding().AutoID == buildingID)
                    {
                        Vre.Server.BusinessLogic.Suite strippedSuite = new
                            Vre.Server.BusinessLogic.Suite(null, 0, suite.FloorNumber, suite.Name);
                        strippedSuite.UpdateFromClient(suite.suite.ClientData);
                        units.Add(strippedSuite.GetClientData());
                        unitsToUpdate.Add(suite);
                        if (units.Count >= MaxNumberOfRecordsInMessage)
                            break;
                    }
                }
            }

            Vre.Server.BusinessLogic.ClientData changes = new Vre.Server.BusinessLogic.ClientData();
            changes.Add("suites", units.ToArray());

            return changes;
        }

        public bool changed
        {
            get
            {
                return PriceChanged ||
                       StatusChanged ||
                       ShowPanoramicViewChanged;
            }
        }
        protected bool promoted { get { return _promoted; } }

        static public void Clear()
        {
            m_changedSuites.Clear();
        }

        static public void checkIn(ChangingSuite candidate)
        {
            if (m_changedSuites.ContainsKey(candidate.suite.UniqueKey))
            {
                m_changedSuites[candidate.suite.UniqueKey].ChangeFrom(candidate);
                return;
            }

            m_changedSuites.Add(candidate.suite.UniqueKey, candidate);
        }

        static public string GenerateChangesReport(List<ChangingSuite> unitsToUpdate,
                                                   bool addDateTimeInfo)
        {
            string ret = string.Empty;
            string dateTimePrefix = addDateTimeInfo ? DateTime.Now.ToString() + " > " : string.Empty;
            int count = 0;
            foreach (var c in unitsToUpdate)
            {
                if (c.changed)
                {
                    ret += string.Format("{0}Suite {1} changed: {2}{3}",
                        dateTimePrefix, c.suite.UniqueKey, c.WhatChanged, System.Environment.NewLine);
                    count++;
                }
                //if (c.promoted)
                //    ret += string.Format("{0}Suite {1} has been promoted\n",
                //        dateTimePrefix, c.suite.UniqueKey);
            }
            ret += string.Format("    {0} changes in this package{1}", 
                                count, System.Environment.NewLine);
            return ret;
        }

        static public string GenerateChangesReport(bool addDateTimeInfo)
        {
            return GenerateChangesReport(m_changedSuites.Values.ToList(), addDateTimeInfo);
        }

        public bool PriceChanged { get { return base.Price != _changed.Price; } }
        public bool StatusChanged { get { return base.Status != _changed.Status; } }
        public bool ShowPanoramicViewChanged { get { return base.ShowPanoramicView != _changed.ShowPanoramicView; } }

        public string WhatChanged
        {
            get
                {
                    List<string> diffs = new List<string>();
                    if (PriceChanged)
                        diffs.Add("Price: (" + base.getPriceString() + "->" + _changed.getPriceString() + ")");

                    if (StatusChanged)
                        diffs.Add("Status: (" + base.Status + "->" + _changed.Status + ")");

                    if (ShowPanoramicViewChanged)
                        diffs.Add("View: (" + base.ShowPanoramicView + "->" + _changed.ShowPanoramicView + ")");

                    if (diffs.Count == 0)
                        return string.Empty;

                    string ret = string.Empty;
                    for (int i=0; i<diffs.Count; i++)
                    {
                        ret += diffs[i];
                        if (i<(diffs.Count-1))
                            ret += ", ";
                    }
                    return ret;
                }
        }

        private void ChangeFrom(ChangingSuite from)
        {
            _changed = from._changed;
            if (!_promoted)
                _promoted = from.promoted;
        }

        static public void MergeChanges(List<ChangingSuite> allSuites)
        {
            foreach (var suite in allSuites)
            {
                ChangingSuite changedSuite = null;
                if (m_changedSuites.TryGetValue(suite.suite.UniqueKey, out changedSuite))
                    suite.ChangeFrom(changedSuite);
            }
        }

        public delegate void OnPromotionDelegate(ChangingSuite suite);

        static public void PromoteChanges(OnPromotionDelegate updateDelegate,
                                          List<ChangingSuite> unitsToUpdate)
        {
            foreach (var c in unitsToUpdate)
                c.PromoteChange(updateDelegate);
        }

        private void PromoteChange(OnPromotionDelegate updateDelegate)
        {
            _promoted = true;
            string baseCSV = _changed.ToCSV();
            base.FromCSV(baseCSV);
            if (updateDelegate != null)
                updateDelegate(this);
        }

        static public void DiscardChanges()
        {
            foreach (var c in m_changedSuites.Values)
            {
                c.DiscardChange();
            }
        }

        public void DiscardChange()
        {
            _changed = base.MakeClone();
        }

        public override string ToString()
        {
            string prefix = string.Empty;
            if (this.changed)
                prefix += "+";
            else
                prefix += "  ";

            if (this.promoted)
                prefix += "> ";
            else
                prefix += "   ";

            return prefix + suite.ToString();
        }

        public override string[] ToStringArray()
        {
            int baseLength = suite.ToStringArray().Length;
            string[] baseArray = suite.ToStringArray();
            string[] retArray = new string[baseLength + 1];
            string prefix = string.Empty;
            if (this.changed)
                prefix += "+";
            else
                prefix += "  ";

            if (this.promoted)
                prefix += "> ";
            else
                prefix += "   ";
            retArray[0] = prefix;
            for (int i = 0; i < baseLength; i++)
                retArray[i + 1] = baseArray[i];

            return retArray;
        }

        public int CompareTo(object obj)
        {
            ChangingSuite that = obj as ChangingSuite;
            return suite.CompareTo(that.suite);
        }

        public bool Equals(ChangingSuite that)
        {
            return suite.Equals(that.suite);
        }

        public override bool Equals(Object obj)
        {
            ChangingSuite thatSuite = obj as ChangingSuite;
            if (thatSuite != null)
                return this.Equals(thatSuite);

            string thatStr = obj.ToString();
            if (thatStr != null)
                return ToString().Equals(thatStr);

            return false;
        }


    } // ChangingSuite
}
