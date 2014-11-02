using System.Collections.Generic;
using Vre.Server.BusinessLogic;
using System;

namespace Vre.Server.Mls
{
    public class MlsItem
    {
        public enum SaleLease { Unknown, Sale, Lease }

        public string MlsId;

        public string SuiteName;
		public string StreetNumber;
		public string StreetName;
		public string StreetType;
		public string StreetDirection;
		public string Municipality;
        public string StateProvince;
        public string PostalCode;
	    public string CompiledAddress;

        public int BedroomCount { get; private set; }
        public int OtherRoomCount { get; private set; }
        public int WashroomCount { get; private set; }
        public int BalconyCount { get; private set; }
        public int TerraceCount { get; private set; }
        
        public int FloorAreaSqFt { get; private set; }

        public string VTourUrl;

	    public SaleLease SaleLeaseState;
	    public char Status;
        public double CurrentPrice;
		public Currency PriceCurrency; 

		public readonly Dictionary<string, string> RawData = new Dictionary<string, string>();

        public void SetSuite(Suite s)
        {
        }

        public void SetSuiteType(SuiteType st)
        {
        }
    }
}