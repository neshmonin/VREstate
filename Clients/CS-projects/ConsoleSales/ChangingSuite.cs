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

        public ChangingSuite(Vre.Server.BusinessLogic.Client.SuiteEx suite)
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
        
        static public Vre.Server.BusinessLogic.ClientData GenerateClientData()
        {
            List<Vre.Server.BusinessLogic.ClientData> units = new List<Vre.Server.BusinessLogic.ClientData>();

            foreach (var suite in m_changedSuites.Values)
            {
                if (suite.changed)
                    units.Add(suite.suite.ClientData);
            }

            Vre.Server.BusinessLogic.ClientData changes = new Vre.Server.BusinessLogic.ClientData();
            changes.Add("suites", units.ToArray());

            return changes;
        }

        public bool changed
        {
            get
            {
                return base.Price != _changed.Price ||
                       base.Status != _changed.Status ||
                       base.ShowPanoramicView != _changed.ShowPanoramicView ||
                       base.CellingHeight != _changed.CellingHeight;
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

        static public string GenerateChangesReport(bool addDateTimeInfo)
        {
            string ret = string.Empty;
            string dateTimePrefix = addDateTimeInfo ? DateTime.Now.ToString() + " > " : string.Empty;
            foreach (var c in m_changedSuites.Values)
            {
                if (c.changed)
                    ret += string.Format("{0}Suite {1} changed: {2}\n",
                        dateTimePrefix, c.suite.UniqueKey, c.WhatChanged);
                //if (c.promoted)
                //    ret += string.Format("{0}Suite {1} has been promoted\n",
                //        dateTimePrefix, c.suite.UniqueKey);
            }
            return ret;
        }

        public string WhatChanged
        {
            get
                {
                    List<string> diffs = new List<string>();
                    if (base.Price != _changed.Price)
                        diffs.Add("Price: (" + base.getPriceString() + "->" + _changed.getPriceString() + ")");

                    if (base.Status != _changed.Status)
                        diffs.Add("Status: (" + base.Status + "->" + _changed.Status + ")");

                    if (base.ShowPanoramicView != _changed.ShowPanoramicView)
                        diffs.Add("View: (" + base.ShowPanoramicView + "->" + _changed.ShowPanoramicView + ")");

                    if (base.CellingHeight != _changed.CellingHeight)
                        diffs.Add("Height: (" + base.CellingHeight + "->" + _changed.CellingHeight + ")");

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

        static public void PromoteChanges(OnPromotionDelegate updateDelegate)
        {
            foreach (var c in m_changedSuites.Values)
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
