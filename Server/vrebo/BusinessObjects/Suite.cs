using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
	[Serializable]
	public partial class Suite : UpdateableBase
	{
	    public enum SalesStatus : byte
	    {
            Available = 0,
            OnHold = 1,
            Sold = 2,
            ResaleAvailable = 3
	    }

        public virtual int PhysicalLevelNumber { get; set; }
        public virtual string FloorName { get; set; }
        public virtual string SuiteName { get; set; }
        public virtual SalesStatus Status { get; set; }

        public virtual ICollection<Option> OptionsPossible { get; protected set; }
        //public IList<VRTour> VRTours { get; protected set; }

        public virtual Building Building { get; set; }
        public virtual SuiteType SuiteType { get; set; }
        public virtual ViewPoint Location { get; set; }
        public virtual ValueWithUM CeilingHeight { get; set; }
        public virtual bool ShowPanoramicView { get; set; }

        public virtual string BubbleTemplateUrl { get; set; }

        /// <summary>
        /// Seller agent reference
        /// </summary>
        public virtual User SellingBy { get; set; }

        /// <summary>For NHibernate; NOT USED: value set by Model Cache</summary>
        protected virtual string ceilingHeight { get { return CeilingHeight.AsRaw; } set { CeilingHeight = new ValueWithUM(value); } }

        protected Suite() { }

        protected Suite(Suite copy) : base(copy)
        {
            PhysicalLevelNumber = copy.PhysicalLevelNumber;
            FloorName = copy.FloorName;
            SuiteName = copy.SuiteName;
            Status = copy.Status;
            OptionsPossible = copy.OptionsPossible;
            Building = copy.Building;
            SuiteType = copy.SuiteType;
            Location = copy.Location;
            CeilingHeight = new ValueWithUM(copy.CeilingHeight.ValueAs(ValueWithUM.Unit.Feet), ValueWithUM.Unit.Feet);
            ShowPanoramicView = copy.ShowPanoramicView;
        }

        public Suite(Building building, int physicalLevelNumber, string floorName, string suiteName)
            : base()
		{
            InitializeNew();
            Building = building;
            PhysicalLevelNumber = physicalLevelNumber;
            FloorName = floorName;
            SuiteName = suiteName;
            Status = SalesStatus.Available;
            Location = ViewPoint.Empty;
            CeilingHeight = ValueWithUM.EmptyLinear;
            ShowPanoramicView = true;
            OptionsPossible = new HashSet<Option>();
            if (Building != null) Building.Suites.Add(this);
        }

        public Suite(ClientData fromServer, Building building)
            : this(building, 0, string.Empty, string.Empty)
        {
            AutoID = fromServer.GetProperty("id", -1);

            int suiteTypeId = fromServer.GetProperty("suiteTypeId", 0);
            Boolean found = false;            
            foreach (SuiteType suiteType in building.ConstructionSite.SuiteTypes)
            {
                if (suiteType.AutoID == suiteTypeId)
                {
                    SuiteType = suiteType;  // informational only
                    found = true;
                    break;
                }
            }
            if (!found)
                SuiteType = new BusinessLogic.SuiteType(building.ConstructionSite, "<unknown>");

            bool changed = UpdateFromClient(fromServer);

            FloorName = fromServer.UpdateProperty("floorName", FloorName, ref changed);
            if (CeilingHeight != null)
                CeilingHeight.SetValue(fromServer.UpdateProperty("ceilingHeightFt", CeilingHeight.ValueAs(ValueWithUM.Unit.Feet), ref changed), ValueWithUM.Unit.Feet);
        }

        public Suite(ClientData fromServer) : this(fromServer, null) {}

        public override ClientData GetClientData()
        {
            ClientData result = base.GetClientData();

            result.Add("buildingId", Building.AutoID);  // informational only

            result.Add("levelNumber", PhysicalLevelNumber);
            result.Add("floorName", FloorName);
            result.Add("name", SuiteName);
            if (CeilingHeight != null)
                result.Add("ceilingHeightFt", CeilingHeight.ValueAs(ValueWithUM.Unit.Feet));
            result.Add("showPanoramicView", ShowPanoramicView);
            result.Add("status", ClientData.ConvertProperty<SalesStatus>(Status));

            if (Location != null)
                result.Add("position", Location.GetClientData());

            if (SuiteType != null)
                result.Add("suiteTypeId", SuiteType.AutoID);  // informational only

            if (SellingBy != null)
                result.Add("sellerId", SellingBy.AutoID);

            if (!string.IsNullOrEmpty(BubbleTemplateUrl))
                result.Add("bubbleTemplateUrl", BubbleTemplateUrl);

            return result;
        }

        public virtual ClientData GetInventoryClientData(ClientData result, bool supplement)
        {
            if (null == result) result = new ClientData();
            if (!supplement)
            {
                result.Add("id", AutoID);
                result.Add("name", SuiteName);
            }

            result.Add("levelNumber", PhysicalLevelNumber);
            result.Add("floorName", FloorName);

            result.Add("status", ClientData.ConvertProperty<SalesStatus>(Status));

            if (SuiteType != null)
                SuiteType.GetInventoryClientData(result, true);

            if (!string.IsNullOrEmpty(BubbleTemplateUrl))
                result.Add("bubbleTemplateUrl", BubbleTemplateUrl);

            return result;
        }

        public override bool UpdateFromClient(ClientData data)
        {
            bool changed = base.UpdateFromClient(data);

            PhysicalLevelNumber = data.UpdateProperty("levelNumber", PhysicalLevelNumber, ref changed);
            SuiteName = data.UpdateProperty("name", SuiteName, ref changed);
            ShowPanoramicView = data.UpdateProperty("showPanoramicView", ShowPanoramicView, ref changed);
            Status = data.UpdateProperty<SalesStatus>("status", Status, ref changed);

            return changed;
        }

        public override string ToString()
        {
            return SuiteName;
        }
    }
}
