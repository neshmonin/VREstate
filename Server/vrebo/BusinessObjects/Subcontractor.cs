using System;

namespace Vre.Server.BusinessLogic
{
	[Serializable]
	public partial class Subcontractor : User
	{
        public ContactInfo ContactInfo { get; private set; }
        public Subcontractor()
		{
		}
		public Subcontractor(ContactInfo contactInfo)
			:base()
		{
			ContactInfo = contactInfo;
		}
	}
}
