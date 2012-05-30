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
            Sold = 3
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
        public virtual string Model { get; set; }
        public virtual GeoPoint Location { get; set; }
        public virtual double AltitudeAdjustment { get; set; }
        public virtual GeoPoint Center { get; set; }
        public virtual double MaxSuiteAltitude { get; set; }

        public virtual IList<Suite> Suites { get; protected set; }

        protected Building() { }

		public Building(Site constructionSite, string name)
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
        }

        public Building(ClientData fromServer) : this(null, string.Empty)
        {
            AutoID = fromServer.GetProperty("id", -1);

            UpdateFromClient(fromServer);
        }

        public override ClientData GetClientData()
        {
            ClientData result = new ClientData();

            result.Add("id", AutoID);
            result.Add("deleted", Deleted);

            result.Add("name", Name);
            result.Add("status", Status);
            result.Add("openingDate", OpeningDate);
            result.Add("altitudeAdjustment", AltitudeAdjustment);
            result.Add("maxSuiteAltitude", MaxSuiteAltitude);

            if (Location != null) result.Add("position", Location.GetClientData());

            if (Center != null) result.Add("center", Center.GetClientData());

            return result;
        }

        public override bool UpdateFromClient(ClientData data)
        {
            bool changed = false;

            Name = data.UpdateProperty("name", Name, ref changed);
            Status = data.UpdateProperty<BuildingStatus>("status", Status, ref changed);
            OpeningDate = data.UpdateProperty("openingDate", OpeningDate, ref changed);

            if (Location.UpdateFromClient(data.GetNextLevelDataItem("position"))) changed = true;

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
