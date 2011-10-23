using System;

namespace Vre.Server.BusinessLogic
{
	[Serializable]
	public partial class Price 
	{
        public int AutoID { get; private set; }

        public Option Option { get; private set; }

        public long OriginalCurrencyCents { get; set; }
        public float PricePerUnitForSubcontractor { get; set; }
        public float PricePerUnitForBuyer { get; set; }
        public string UnitName { get; set; }
        public float NumberOfUnits { get; set; }
        public DateTime StartingDate { get; set; }

        private Price() { }

        public Price(Option option)
        {
            Option = option;
            OriginalCurrencyCents = 0;
            PricePerUnitForBuyer = 0.0f;
            PricePerUnitForSubcontractor = 0.0f;
            NumberOfUnits = 0;
            StartingDate = DateTime.UtcNow;
        }
	}
}
