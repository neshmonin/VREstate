using System;

namespace Vre.Server.BusinessLogic
{
	[Serializable]
	public partial class Structure : UpdateableBase
	{
		public virtual string Name { get; set; }
		public virtual string LocalizedName { get; set; }
		public virtual string DisplayModelUrl { get; set; }
        public virtual GeoPoint Location { get; set; }
        public virtual double AltitudeAdjustment { get; set; }

        protected Structure() { }

		public Structure(string name) : base()
		{
            InitializeNew();
			Name = name;
			LocalizedName = name;
			DisplayModelUrl = null;
			Location = GeoPoint.Empty;
            AltitudeAdjustment = 0.0;
        }

        public Structure(ClientData fromServer)
            : base(fromServer)
        {
            LocalizedName = Name = fromServer.GetProperty("name", string.Empty);
            DisplayModelUrl = fromServer.GetProperty("displayModelUrl", string.Empty);
			Location.UpdateFromClient(fromServer.GetNextLevelDataItem("position"));

            UpdateFromClient(fromServer);
        }

        public override ClientData GetClientData()
        {
            ClientData result = base.GetClientData();

			result.Add("name", LocalizedName);
            result.Add("altitudeAdjustment", AltitudeAdjustment);
            if (Location != null) result.Add("position", Location.GetClientData());
            if (!string.IsNullOrEmpty(DisplayModelUrl))
                result.Add("displayModelUrl", DisplayModelUrl);

            return result;
        }

        public override bool UpdateFromClient(ClientData data)
        {
            bool changed = base.UpdateFromClient(data);

            AltitudeAdjustment = data.UpdateProperty("altitudeAdjustment", AltitudeAdjustment, ref changed);

            return changed;
        }

        public override string ToString()
        {
            return AutoID.ToString();
        }
    }
}
