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
            ResaleAvailable = 3,
            AvailableRent = 4
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

		public virtual Money? CurrentPrice { get; set; }
		protected virtual string currentPriceDb
		{
			get { return (CurrentPrice.HasValue ? CurrentPrice.Value.ToFullString() : null); }
			set { Money m; if (Money.TryParse(value, out m)) CurrentPrice = m; else CurrentPrice = null; }
		}

		public virtual string BubbleTemplateUrl { get; set; }

        /// <summary>
        /// Seller agent reference
        /// </summary>
        public virtual User SellingBy { get; set; }

		/// <summary>for use by NHibernate</summary>
        protected virtual string ceilingHeight 
		{ 
			get { return (null == CeilingHeight) ? null : CeilingHeight.AsRaw; } 
			set { CeilingHeight = (null == value) ? null : new ValueWithUM(value); } 
		}

        protected Suite() { }

        public Suite(Suite copy)
            : base(copy)
        {
            PhysicalLevelNumber = copy.PhysicalLevelNumber;
            FloorName = copy.FloorName;
            SuiteName = copy.SuiteName;
            Status = copy.Status;
            OptionsPossible = copy.OptionsPossible;
            Building = copy.Building;
            SuiteType = copy.SuiteType;
            Location = copy.Location;
            CeilingHeight = (copy.CeilingHeight != null) ? new ValueWithUM(copy.CeilingHeight.ValueAs(ValueWithUM.Unit.Feet), ValueWithUM.Unit.Feet) : null;
	        CurrentPrice = copy.CurrentPrice;
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
	        CurrentPrice = new Money();
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
            if (building != null && building.ConstructionSite != null)
            {
                foreach (SuiteType suiteType in building.ConstructionSite.SuiteTypes)
                {
                    if (suiteType.AutoID == suiteTypeId)
                    {
                        SuiteType = suiteType;  // informational only
                        found = true;
                        break;
                    }
                }
            }
            if (!found)
                SuiteType = new BusinessLogic.SuiteType(building.ConstructionSite, "<unknown>");

            bool changed = UpdateFromClient(fromServer);

            FloorName = fromServer.UpdateProperty("floorName", FloorName, ref changed);
            if (CeilingHeight != null)
                CeilingHeight.SetValue(fromServer.UpdateProperty("ceilingHeightFt", CeilingHeight.ValueAs(ValueWithUM.Unit.Feet), ref changed), ValueWithUM.Unit.Feet);

	        decimal cp = fromServer.GetProperty("currentPrice", -1.0m);
	        Currency cpc;
			if ((cp >= 0.0m) && Currency.TryParse(fromServer.GetProperty("currentPriceCurrency", string.Empty), out cpc))
				CurrentPrice = new Money(cp, cpc);
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

	        if (CurrentPrice.HasValue)
	        {
		        result.Add("currentPrice", CurrentPrice.Value.ToString("F"));
				result.Add("currentPriceDisplay", CurrentPrice.Value.ToString("C"));
				result.Add("currentPriceCurrency", CurrentPrice.Value.Currency.Iso3LetterCode);
	        }

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

			if (CurrentPrice.HasValue)
			{
				result.Add("currentPrice", CurrentPrice.Value.ToString("F"));
				result.Add("currentPriceCurrency", CurrentPrice.Value.Currency.Iso3LetterCode);
			}

			return result;
        }

        public override bool UpdateFromClient(ClientData data)
        {
            bool changed = base.UpdateFromClient(data);

            PhysicalLevelNumber = data.UpdateProperty("levelNumber", PhysicalLevelNumber, ref changed);
            SuiteName = data.UpdateProperty("name", SuiteName, ref changed);
            ShowPanoramicView = data.UpdateProperty("showPanoramicView", ShowPanoramicView, ref changed);
            Status = data.UpdateProperty("status", Status, ref changed);
			updatePrice(data, ref changed);

			return changed;
        }

		public virtual bool UpdateFromClient(ClientData data, ICollection<string> availableFields, out bool changesSkipped)
		{
			changesSkipped = false;
			bool result = base.UpdateFromClient(data);

			if (availableFields.Contains("levelNumber")) PhysicalLevelNumber = data.UpdateProperty("levelNumber", PhysicalLevelNumber, ref result);
			else if (data.GetProperty("levelNumber", 0) != PhysicalLevelNumber) changesSkipped = true;

			if (availableFields.Contains("name")) SuiteName = data.UpdateProperty("name", SuiteName, ref result);
			else if (data.GetProperty("name", string.Empty) != SuiteName) changesSkipped = true;

			if (availableFields.Contains("showPanoramicView")) ShowPanoramicView = data.UpdateProperty("showPanoramicView", ShowPanoramicView, ref result);
			else if (data.GetProperty("showPanoramicView", false) != ShowPanoramicView) changesSkipped = true;

			if (availableFields.Contains("status")) Status = data.UpdateProperty("status", Status, ref result);
			else if (data.GetProperty<SalesStatus>("status", SalesStatus.Available) != Status) changesSkipped = true;

			if (availableFields.Contains("currentPrice"))
			{
				updatePrice(data, ref result);
			}
			else
			{
				if (CurrentPrice.HasValue)
				{
					if (!CurrentPrice.Value.Equals(new Money(
							data.GetProperty("currentPrice", Convert.ToDecimal(CurrentPrice.Value)),
							CurrentPrice.Value.Currency)))
						changesSkipped = true;
				}
				else
				{
					if (data.GetProperty("currentPrice", -1.0m) > 0.0m) changesSkipped = true;
				}
			}

			return result;
		}

		private void updatePrice(ClientData data, ref bool result)
		{
			if (CurrentPrice.HasValue)
			{
				CurrentPrice = new Money(
					data.UpdateProperty("currentPrice", Convert.ToDecimal(CurrentPrice.Value), ref result),
					CurrentPrice.Value.Currency);
			}
			else
			{
				var value = data.GetProperty("currentPrice", -1.0m);
				if (value >= 0.0m)
				{
					Currency c;
					if (!Currency.TryParse(data.GetProperty("currentPriceCurrency", "CAD"), out c))
						c = Currency.Cad;

					CurrentPrice = new Money(value, c);
					result = true;
				}
			}
		}
		
		public override string ToString()
        {
            return SuiteName;
        }
    }
}
