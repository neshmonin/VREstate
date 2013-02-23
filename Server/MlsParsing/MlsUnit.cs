using Vre.Server.BusinessLogic;
namespace Vre.Server.Mls
{
    public class MlsUnit
    {
        public enum SaleLease { Unknown, Sale, Lease }

        public string MlsId { get; private set; }

        public string SuiteName { get; private set; }
        public string StreetAddress { get; private set; }
        public string Municipality { get; private set; }

        public int BedroomCount { get; private set; }
        public int OtherRoomCount { get; private set; }
        public int WashroomCount { get; private set; }
        public int BalconyCount { get; private set; }
        public int TerraceCount { get; private set; }
        
        public int FloorAreaSqFt { get; private set; }

        public string VTourUrl { get; private set; }

        public SaleLease SaleLeaseState { get; private set; }
        public char Status { get; private set; }
        public double CurrentPrice { get; private set; }


        public void SetSuite(Suite s)
        {
        }

        public void SetSuiteType(SuiteType st)
        {
        }
    }
}