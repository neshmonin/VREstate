using System;

namespace Vre.Server.BusinessLogic
{
	[Serializable]
	public partial class Account 
	{
        public int AutoID { get; private set; }
        public int SuiteID { get; private set; }
        public int BuyerID { get; private set; }
        public float CurrentBalance { get; set; }
        public long CurrencyCents { get; set; }

		private Account() { }

        public Account(int suiteId, int buyerId)
        {
            SuiteID = suiteId;
            BuyerID = buyerId;
            CurrentBalance = 0.0f;
            CurrencyCents = 0;
        }
	}
}
