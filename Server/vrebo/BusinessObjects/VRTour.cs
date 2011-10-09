using System;

namespace Vre.Server.BusinessLogic
{
	[Serializable]
	public partial class VRTour 
	{
        private int AutoID { get; set; }

        public DateTime StartedOn { get; private set; }
        public int SuiteID { get; private set; }
        public int BuyerID { get; private set; }
        public int SalespersonID { get; private set; }
        public string TourRecording { get; private set; }

		public VRTour()
		{
		}

		public VRTour(int suiteID, int buyerID, int salespersonID, string tourRecording)
		{
            StartedOn = DateTime.UtcNow;
			SuiteID = suiteID;
			BuyerID = buyerID;
			SalespersonID = salespersonID;
			TourRecording = tourRecording;
		}
	}
}
