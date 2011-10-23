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
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
