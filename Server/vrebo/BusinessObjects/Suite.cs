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

        public virtual IList<Option> OptionsPossible { get; protected set; }
        //public IList<VRTour> VRTours { get; protected set; }

        public virtual Building Building { get; protected set; }
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
            CeilingHeight = copy.CeilingHeight;
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
            OptionsPossible = new List<Option>();
            if (Building != null) Building.Suites.Add(this);
        }

        public Suite(ClientData fromServer) : this(null, 0, string.Empty, string.Empty)
        {
            AutoID = fromServer.GetProperty("id", -1);

            SuiteType = new BusinessLogic.SuiteType(null, fromServer.GetProperty("suiteTypeName", string.Empty));  // informational only

            UpdateFromClient(fromServer);
        }

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
