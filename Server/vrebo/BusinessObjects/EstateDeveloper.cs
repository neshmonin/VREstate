using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
	[Serializable]
	public partial class EstateDeveloper : UpdateableBase
	{
	    public enum Configuration : byte
	    {
            Kiosk_SingleScreen,
            Kiosk_SingleOffice_MultipleScreens,
            Kiosk_MultipleOffices,
            Online
	    }

        public virtual IList<Site> Sites { get; protected set; }
        public virtual string Name { get; set; }
        public Configuration VREConfiguration { get; private set; }

		private EstateDeveloper() { }

        public EstateDeveloper(Configuration vreConfiguration)
        {
            InitializeNew();
            VREConfiguration = vreConfiguration;
            Sites = new List<Site>();
        }

        public EstateDeveloper(ClientData data) : base(data) 
        {
            Sites = new List<Site>();
            UpdateFromClient(data);
        }

        public override bool UpdateFromClient(ClientData data)
        {
            bool result = base.UpdateFromClient(data);

            Name = data.UpdateProperty("name", Name, ref result);
            VREConfiguration = data.UpdateProperty("configuration", VREConfiguration, ref result);

            return result;
        }
        
        public override string ToString()
        {
            return Name;
        }

        public override ClientData GetClientData()
        {
            ClientData result = new ClientData();

            result.Add("id", AutoID);  // informational only
            result.Add("deleted", Deleted);  // informational only

            result.Add("name", Name);  // informational only
            result.Add("configuration", VREConfiguration);  // informational only

            return result;
        }
    }
}
