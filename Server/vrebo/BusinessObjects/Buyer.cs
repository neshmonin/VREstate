using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
	[Serializable]
	public partial class Buyer : User
	{
        public IList<Account> Accounts { get; private set; }
        public IList<VRTour> VRTours { get; private set; }
        public ContactInfo ContactInfo { get; private set; }

		public Buyer()
		{
		}
		
        public Buyer(IList<Account> accounts, IList<VRTour> vrTours, ContactInfo contactInfo)
			: base()
		{
			Accounts = accounts;
			VRTours = vrTours;
			ContactInfo = contactInfo;
		}		
	}
}
