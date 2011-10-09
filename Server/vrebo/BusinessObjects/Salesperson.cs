using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
	[Serializable]
	public partial class Salesperson : User
	{
        public IList<VRTour> VRTours { get; private set; }
        public ContactInfo ContactInfo { get; private set; }
        public Salesperson()
		{
		}
		public Salesperson(IList<VRTour> vrTours, ContactInfo contactInfo)
			:base()
		{
			VRTours = vrTours;
			ContactInfo = contactInfo;
		}
	}
}
