using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
	[Serializable]
	public partial class Suite : UpdateableBase
	{
	    public enum SalesStatus
	    {
            Available,
            OnHold,
            Sold
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
        public virtual Wireframe[] Model { get; set; }
        public virtual ValueWithUM CeilingHeight { get; set; }
        public virtual bool ShowPanoramicView { get; set; }

        /// <summary>For NHibernate</summary>
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
            ClientData result = new ClientData();

            result.Add("id", AutoID);  // informational only

            result.Add("levelNumber", PhysicalLevelNumber);
            result.Add("floorName", FloorName);
            result.Add("name", SuiteName);
            result.Add("ceilingHeightFt", CeilingHeight.ValueAs(ValueWithUM.Unit.Feet));
            result.Add("showPanoramicView", ShowPanoramicView);
            result.Add("status", Status);

            if (Model != null)
            {
                int idx = 0;
                ClientData[] model = new ClientData[Model.Length];
                foreach (Wireframe wf in Model) model[idx++] = wf.GetClientData();
                result.Add("geometries", model);  // keep model's terminology
            }
            if (Location != null)
            {
                result.Add("position", Location.GetClientData());
            }

            if (SuiteType != null)
                result.Add("suiteTypeName", SuiteType.Name);  // informational only

            return result;
        }

        public override bool UpdateFromClient(ClientData data)
        {
            bool changed = false;

            PhysicalLevelNumber = data.UpdateProperty("levelNumber", PhysicalLevelNumber, ref changed);
            FloorName = data.UpdateProperty("floorName", FloorName, ref changed);
            SuiteName = data.UpdateProperty("name", SuiteName, ref changed);
            CeilingHeight.SetValue(
                data.UpdateProperty("ceilingHeightFt", CeilingHeight.ValueAs(ValueWithUM.Unit.Feet), ref changed), 
                ValueWithUM.Unit.Feet);
            ShowPanoramicView = data.UpdateProperty("showPanoramicView", ShowPanoramicView, ref changed);
            Status = data.UpdateProperty<SalesStatus>("status", Status, ref changed);

            if (Location.UpdateFromClient(data.GetNextLevelDataItem("position"))) changed = true;

            return changed;
        }

        public override string ToString()
        {
            return SuiteName;
        }
    }
}
