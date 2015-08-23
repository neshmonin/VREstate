using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public partial class SuiteType : UpdateableBase
    {
        public virtual Site ConstructionSite { get; protected set; }
        public virtual string Name { get; set; }
		public virtual string LocalizedName { get; set; }
		public virtual string WireframeModel { get; set; }
        public virtual IList<SuiteLevel> Levels { get; protected set; }

        public virtual ValueWithUM FloorArea { get; set; }  // TODO: Should be computable field!
        public virtual int BedroomCount { get; set; }  // TODO: Should be computable field!
        public virtual int DenCount { get; set; }  // TODO: Should be computable field!
        public virtual int OtherRoomCount { get; set; }  // TODO: Should be computable field!
        public virtual int ShowerBathroomCount { get; set; }  // TODO: Should be computable field!
        public virtual int NoShowerBathroomCount { get; set; }  // TODO: Should be computable field!
        public virtual int BalconyCount { get; set; }  // TODO: Should be computable field!
        public virtual int TerraceCount { get; set; }  // TODO: Should be computable field!

        public virtual string FloorPlanUrl { get; set; }

        /// <summary>
        /// Not used; required for proper inverse mapping and updating references from suites
        /// </summary>
        protected virtual IList<Suite> SuitesInvolved { get; set; }

        protected virtual string floorArea { get { return FloorArea.AsRaw; } set { FloorArea = new ValueWithUM(value); } }

        protected SuiteType() { }

        public SuiteType(Site constructionSite, string name) : base()
        {
            InitializeNew();
            ConstructionSite = constructionSite;
            Name = name;
			LocalizedName = name;
            Levels = new List<SuiteLevel>();
            WireframeModel = null;
            SuitesInvolved = new List<Suite>();
            if (constructionSite != null) constructionSite.SuiteTypes.Add(this);
        }

        public SuiteType(ClientData data, Site constructionSite) : base(data)
        {
            ConstructionSite = constructionSite;
            Levels = new List<SuiteLevel>();
            WireframeModel = null;
            SuitesInvolved = new List<Suite>();
            UpdateFromClient(data);
            if (constructionSite != null)
                constructionSite.SuiteTypes.Add(this);

            double area = data.GetProperty("area", 0.0);
            FloorArea = new ValueWithUM(area, ValueWithUM.Unit.SqFeet);
        }

        public SuiteType(ClientData data) : base(data) { }

        public virtual void debug_addSuite(Suite s)
        {
            s.SuiteType = this;
            SuitesInvolved.Add(s);
        }

        public override bool UpdateFromClient(ClientData data)
        {
            bool result = false;

			LocalizedName = data.UpdateProperty("name", LocalizedName, ref result);
            //Model = data.UpdateProperty("modelName", Model, ref changed);
            BedroomCount = data.UpdateProperty("bedrooms", BedroomCount, ref result);
            DenCount = data.UpdateProperty("dens", DenCount, ref result);
            OtherRoomCount = data.UpdateProperty("otherRooms", OtherRoomCount, ref result);
            ShowerBathroomCount = data.UpdateProperty("showerBathrooms", ShowerBathroomCount, ref result);
            NoShowerBathroomCount = data.UpdateProperty("noShowerBathrooms", NoShowerBathroomCount, ref result);
            BalconyCount = data.UpdateProperty("balconies", BalconyCount, ref result);
            TerraceCount = data.UpdateProperty("terraces", TerraceCount, ref result);

            return result;
        }

        public virtual float Bathrooms
        {
            get
            {
                return (NoShowerBathroomCount <= 1)
                ? ((float)ShowerBathroomCount + 0.5f * (float)NoShowerBathroomCount)
                : (float)(ShowerBathroomCount + NoShowerBathroomCount - 1) + 0.5f;
            }
        }

        public virtual string FloorAreaString
        {
            get
            {
                if (FloorArea != null)
                {
                    switch (FloorArea.StoredAs)
                    {
                        case ValueWithUM.Unit.SqFeet:
                            return FloorArea.ValueAs(FloorArea.StoredAs).ToString();
                        case ValueWithUM.Unit.SqMeters:
                            return FloorArea.ValueAs(FloorArea.StoredAs).ToString();
                    }
                }
                return "";
            }
        }

        public override ClientData GetClientData()
        {
            ClientData result = base.GetClientData();
            int idx;

            result.Add("siteId", ConstructionSite.AutoID);  // informational only

			result.Add("name", LocalizedName);
            //result.Add("modelName", Model);  // informatinal only

            if (!string.IsNullOrEmpty(FloorPlanUrl))
                result.Add("floorPlanUrl", FloorPlanUrl);

            if (Levels != null)
            {
                idx = 0;
                ClientData[] levels = new ClientData[Levels.Count];
                foreach (SuiteLevel sl in Levels) levels[idx++] = sl.GetClientData();
                result.Add("levels", levels);
            }

            if (WireframeModel != null)
            {
				// TODO: converting text->json->text
				var cd = JavaScriptHelper.JsonToClientData(WireframeModel);
				result.Add("geometries", cd["geometries"]);  // keep model's terminology
            }

            if (FloorArea != null)
            {
                switch (FloorArea.StoredAs)
                {
                    case ValueWithUM.Unit.SqFeet:
                        result.Add("area", FloorArea.ValueAs(FloorArea.StoredAs));
                        result.Add("areaUm", "sqFt");
                        break;

                    case ValueWithUM.Unit.SqMeters:
                        result.Add("area", FloorArea.ValueAs(FloorArea.StoredAs));
                        result.Add("areaUm", "sqM");
                        break;
                }
            }

            result.Add("bedrooms", BedroomCount);
            result.Add("dens", DenCount);
            result.Add("otherRooms", OtherRoomCount);
            result.Add("showerBathrooms", ShowerBathroomCount);
            result.Add("noShowerBathrooms", NoShowerBathroomCount);
            result.Add("bathrooms", Bathrooms);
            result.Add("balconies", BalconyCount);
            result.Add("terraces", TerraceCount);

            return result;
        }

        public virtual ClientData GetInventoryClientData(ClientData result, bool supplement)
        {
            if (null == result) result = new ClientData();
            if (!supplement)
            {
                result.Add("id", AutoID);
				result.Add("name", LocalizedName);
            }

            if (!string.IsNullOrEmpty(FloorPlanUrl))
                result.Add("floorPlanUrl", FloorPlanUrl);

            if (FloorArea != null)
            {
                switch (FloorArea.StoredAs)
                {
                    case ValueWithUM.Unit.SqFeet:
                        result.Add("area", FloorArea.ValueAs(FloorArea.StoredAs));
                        result.Add("areaUm", "sqFt");
                        break;

                    case ValueWithUM.Unit.SqMeters:
                        result.Add("area", FloorArea.ValueAs(FloorArea.StoredAs));
                        result.Add("areaUm", "sqM");
                        break;
                }
            }

            result.Add("bedrooms", BedroomCount);
            result.Add("dens", DenCount);
            result.Add("otherRooms", OtherRoomCount);
            result.Add("showerBathrooms", ShowerBathroomCount);
            result.Add("noShowerBathrooms", NoShowerBathroomCount);
            result.Add("bathrooms", Bathrooms);
            result.Add("balconies", BalconyCount);
            result.Add("terraces", TerraceCount);

            return result;
        }
    }
}
