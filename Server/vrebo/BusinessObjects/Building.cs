using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
	[Serializable]
	public partial class Building : UpdateableBase
	{
	    public enum BuildingStatus : byte
	    {
            InProject = 0,
            Constructing = 1,
            Built = 2,
            Sold = 3,
            ResaleAvailable = 4,
	    }

        public virtual Site ConstructionSite { get; protected set; }

        public virtual string Name { get; set; }
        public virtual string AddressLine1 { get; set; }
        public virtual string AddressLine2 { get; set; }
        public virtual string City { get; set; }
        public virtual string StateProvince { get; set; }
        public virtual string PostalCode { get; set; }
        public virtual string Country { get; set; }

        public virtual DateTime? OpeningDate { get; set; }
        public virtual BuildingStatus Status { get; set; }
        public virtual string DisplayModelUrl { get; set; }
        public virtual string OverlayModelUrl { get; set; }
		[Obsolete("POI is on site level always; this attribute shall disappear in future versions.", true)]
        public virtual string PoiModelUrl { get; set; }
        public virtual string BubbleWebTemplateUrl { get; set; }
        public virtual string BubbleKioskTemplateUrl { get; set; }
        public virtual GeoPoint Location { get; set; }
        public virtual double AltitudeAdjustment { get; set; }
        public virtual GeoPoint Center { get; set; }
        public virtual double MaxSuiteAltitude { get; set; }
        public virtual string InitialView { get; set; }
        public virtual string WireframeLocation { get; set; }

        /// <summary>
        /// Seller agent reference
        /// </summary>
        public virtual User SellingBy { get; set; }

        public virtual IList<Suite> Suites { get; protected set; }

        protected Building() { }

		public Building(Site constructionSite, string name) : base()
		{
            InitializeNew();
            ConstructionSite = constructionSite;
            Name = name;
            Status = BuildingStatus.InProject;
            Location = GeoPoint.Empty;
            Center = GeoPoint.Empty;
            MaxSuiteAltitude = 0.0;
            AltitudeAdjustment = 0.0;
            Suites = new List<Suite>();
            if (constructionSite != null) constructionSite.Buildings.Add(this);
            InitialView = string.Empty;
        }

        public Building(ClientData fromServer, Site site)
            : base(fromServer)
        {
			ConstructionSite = site;

            AltitudeAdjustment = fromServer.GetProperty("altitudeAdjustment", 0.0);
            MaxSuiteAltitude = fromServer.GetProperty("maxSuiteAltitude", 0.0);

            DisplayModelUrl = fromServer.GetProperty("displayModelUrl", string.Empty);
            OverlayModelUrl = fromServer.GetProperty("overlayModelUrl", string.Empty);
            //PoiModelUrl = fromServer.GetProperty("poiModelUrl", string.Empty);
            BubbleWebTemplateUrl = fromServer.GetProperty("bubbleWebTemplateUrl", string.Empty);
            BubbleKioskTemplateUrl = fromServer.GetProperty("bubbleKioskTemplateUrl", string.Empty);

            AddressLine1 = fromServer.GetProperty("addressLine1", string.Empty);
            AddressLine2 = fromServer.GetProperty("addressLine2", string.Empty);
            City = fromServer.GetProperty("city", string.Empty);
            StateProvince = fromServer.GetProperty("stateProvince", string.Empty);
            PostalCode = fromServer.GetProperty("postalCode", string.Empty);
            Country = fromServer.GetProperty("country", string.Empty);

            UpdateFromClient(fromServer);
        }

        public Building(ClientData fromServer) : this(fromServer, null) { }

        public override ClientData GetClientData()
        {
            ClientData result = base.GetClientData();

            result.Add("siteId", ConstructionSite.AutoID);

            result.Add("name", Name);
            result.Add("status", ClientData.ConvertProperty<Building.BuildingStatus>(Status));
            result.Add("openingDate", OpeningDate);
            result.Add("altitudeAdjustment", AltitudeAdjustment);
            result.Add("maxSuiteAltitude", MaxSuiteAltitude);
            result.Add("initialView", InitialView);

            if (Location != null) result.Add("position", Location.GetClientData());

            if (Center != null) result.Add("center", Center.GetClientData());

            if (SellingBy != null)
                result.Add("sellerId", SellingBy.AutoID);

            if (!string.IsNullOrEmpty(DisplayModelUrl))
                result.Add("displayModelUrl", DisplayModelUrl);
            if (!string.IsNullOrEmpty(OverlayModelUrl))
                result.Add("overlayModelUrl", OverlayModelUrl);
            //if (!string.IsNullOrEmpty(PoiModelUrl))
            //    result.Add("poiModelUrl", PoiModelUrl);
            if (!string.IsNullOrEmpty(BubbleWebTemplateUrl))
                result.Add("bubbleWebTemplateUrl", BubbleWebTemplateUrl);
            if (!string.IsNullOrEmpty(BubbleKioskTemplateUrl))
                result.Add("bubbleKioskTemplateUrl", BubbleKioskTemplateUrl);

            if (!string.IsNullOrEmpty(AddressLine1))
                result.Add("addressLine1", AddressLine1);
            if (!string.IsNullOrEmpty(AddressLine2))
                result.Add("addressLine2", AddressLine2);
            if (!string.IsNullOrEmpty(City))
                result.Add("city", City);
            if (!string.IsNullOrEmpty(StateProvince))
                result.Add("stateProvince", StateProvince);
            if (!string.IsNullOrEmpty(PostalCode))
                result.Add("postalCode", PostalCode);
            if (!string.IsNullOrEmpty(Country))
                result.Add("country", Country);

            return result;
        }

        public override bool UpdateFromClient(ClientData data)
        {
            bool changed = base.UpdateFromClient(data);

            Name = data.UpdateProperty("name", Name, ref changed);
            Status = data.UpdateProperty<BuildingStatus>("status", Status, ref changed);
            OpeningDate = data.UpdateProperty("openingDate", OpeningDate, ref changed);

            if ((Location != null) && Location.UpdateFromClient(data.GetNextLevelDataItem("position"))) changed = true;

            InitialView = data.UpdateProperty("initialView", InitialView, ref changed);

            return changed;
        }

        public override string ToString()
        {
            string result = Name;

            if (!string.IsNullOrWhiteSpace(AddressLine1) ||
                !string.IsNullOrWhiteSpace(AddressLine2))
                result += " (" + AddressLine1 + " " + AddressLine2 + ")";

            if (!string.IsNullOrWhiteSpace(StateProvince))
                result += " (" + StateProvince + ")";

            return result;
        }
    }
}
