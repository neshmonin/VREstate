using System;

namespace Vre.Server.BusinessLogic
{
    public class Listing : UpdateableGuidBase
    {
        public enum ListingType : int
        {
            FloorPlan = 0,
            ExternalTour = 1,
            VirtualTour3D = 2
        }

        public enum SubjectType : int
        {
            Suite = 0,
            Building = 1
        }

        public int OwnerId { get; private set; }
        public DateTime ExpiresOn { get; private set; }
        public bool Enabled { get; set; }

        public int RequestCounter { get; private set; }
        public DateTime LastRequestTime { get; private set; }
        
        public ListingType Product { get; private set; }
        public string MlsId { get; private set; }
        public string ProductUrl { get; private set; }
        public SubjectType TargetObjectType { get; private set; }
        public int TargetObjectId { get; private set; }

        private Listing() { }

        public Listing(int ownerId, 
            Listing.ListingType product, string mlsId, 
            SubjectType type, int targetObjectId, string productUrl, DateTime expiresOn)
        {
            InitializeNew();

            AutoID = Guid.NewGuid();

            OwnerId = ownerId;
            Product = product;
            MlsId = mlsId;
            ProductUrl = productUrl;
            TargetObjectType = type;
            TargetObjectId = targetObjectId;
            ExpiresOn = expiresOn;
            Enabled = true;

            RequestCounter = 0;
            LastRequestTime = DateTime.UtcNow;
        }

        public void Update(
            Listing.ListingType product, string mlsId,
            string productUrl, DateTime expiresOn)
        {
            Product = product;
            MlsId = mlsId;
            ProductUrl = productUrl;
            ExpiresOn = expiresOn;

            MarkUpdated();
        }

        public void Touch()
        {
            RequestCounter++;
            LastRequestTime = DateTime.UtcNow;
        }

        public Listing (ClientData data) : base(data)
        {
            Product = data.GetProperty<ListingType>("product", ListingType.FloorPlan);
            MlsId = data.GetProperty("mlsIs", string.Empty);
            ProductUrl = data.GetProperty("productUrl", string.Empty);
            TargetObjectType = data.GetProperty<SubjectType>("targetObjectType", SubjectType.Suite);
            TargetObjectId = data.GetProperty("targetObjectId", -1);
            RequestCounter = data.GetProperty("requestCounter", 0);
            LastRequestTime = data.GetProperty("lastRequestTime", DateTime.MinValue);
        }

        public override ClientData GetClientData()
        {
            // See also Vre.Server.RemoteService.DataService.getListing()

            ClientData result = base.GetClientData();

            result.Add("ownerId", OwnerId);  // informational only
            result.Add("expiresOn", ExpiresOn);
            result.Add("enabled", Enabled);

            result.Add("product", Product);
            result.Add("mlsId", MlsId);
            result.Add("productUrl", ProductUrl);
            result.Add("targetObjectType", TargetObjectType);
            result.Add("targetObjectId", TargetObjectId);

            result.Add("requestCounter", RequestCounter);  // informational (stats) only
            result.Add("lastRequestTime", LastRequestTime);  // informational (stats) only

            return result;
        }

        public override bool UpdateFromClient(ClientData data)
        {
            bool result = base.UpdateFromClient(data);

            Enabled = data.UpdateProperty("enabled", Enabled, ref result);
            ExpiresOn = data.UpdateProperty("expiresOn", ExpiresOn, ref result);
            OwnerId = data.UpdateProperty("ownerId", OwnerId, ref result);

            return result;
        }
    }
}