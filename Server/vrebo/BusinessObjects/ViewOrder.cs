using System;

namespace Vre.Server.BusinessLogic
{
    public class ViewOrder : UpdateableGuidBase
    {
        public enum ViewOrderProduct : int
        {
            PrivateListing = 0,
            PublicListing = 1,
            Building3DLayout = 2
        }

        public enum ViewOrderOptions : int
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
        public bool Imported { get; set; }
        public string Note { get; set; }

        public int RequestCounter { get; private set; }
        public DateTime LastRequestTime { get; private set; }
        public int NotificationsSent { get; set; }

        public ViewOrderProduct Product { get; private set; }
        public ViewOrderOptions Options { get; private set; }
        public string MlsId { get; private set; }
        public string InfoUrl { get; set; }
        public string VTourUrl { get; set; }
        public SubjectType TargetObjectType { get; private set; }
        public int TargetObjectId { get; private set; }

        /// <summary>
        /// NON-PERSISTENT PROPERTY
        /// </summary>
        public string ViewOrderURL { get; set; }

        private ViewOrder() { }

        public ViewOrder(int ownerId, 
            ViewOrderProduct product, ViewOrderOptions options, string mlsId, 
            SubjectType type, int targetObjectId, string productUrl, DateTime expiresOn)
        {
            InitializeNew();

            AutoID = Guid.NewGuid();

            OwnerId = ownerId;
            Product = product;
            Options = options;
            MlsId = mlsId;
            VTourUrl = productUrl;
            TargetObjectType = type;
            TargetObjectId = targetObjectId;
            ExpiresOn = expiresOn;
            Enabled = true;
            Imported = false;

            RequestCounter = 0;
            LastRequestTime = DateTime.UtcNow;
        }

        public void Update(
            ViewOrder.ViewOrderOptions options, string mlsId, string mlsUrl,
            string productUrl, DateTime expiresOn)
        {
            Options = options;
            MlsId = mlsId;
            InfoUrl = mlsUrl;
            VTourUrl = productUrl;
            ExpiresOn = expiresOn;

            MarkUpdated();
        }

        public void Touch()
        {
            RequestCounter++;
            LastRequestTime = DateTime.UtcNow;
        }

        public void Prolong(DateTime expiresOn)
        {
            ExpiresOn = expiresOn;
            MarkUpdated();
        }

        public ViewOrder(ClientData data)
            : base(data)
        {
            TargetObjectType = data.GetProperty<SubjectType>("targetObjectType", SubjectType.Suite);
            TargetObjectId = data.GetProperty("targetObjectId", -1);
            RequestCounter = data.GetProperty("requestCounter", 0);
            LastRequestTime = data.GetProperty("lastRequestTime", DateTime.MinValue);
            ViewOrderURL = data.GetProperty("viewOrder-url", string.Empty);
        }

        public override ClientData GetClientData()
        {
            // See also Vre.Server.RemoteService.DataService.getViewOrder()

            ClientData result = base.GetClientData();

            result.Add("ownerId", OwnerId);  // informational only
            result.Add("expiresOn", ExpiresOn);
            result.Add("enabled", Enabled);
			result.Add("imported", Imported);
			result.Add("note", Note);

            result.Add("product", ClientData.ConvertProperty<ViewOrderProduct>(Product));
            result.Add("options", ClientData.ConvertProperty<ViewOrderOptions>(Options));
            result.Add("mlsId", MlsId);
            result.Add("infoUrl", InfoUrl);
            result.Add("vTourUrl", VTourUrl);
            result.Add("targetObjectType", ClientData.ConvertProperty<SubjectType>(TargetObjectType));
            result.Add("targetObjectId", TargetObjectId);

            result.Add("requestCounter", RequestCounter);  // informational (stats) only
            result.Add("lastRequestTime", LastRequestTime);  // informational (stats) only

            result.Add("viewOrder-url", ViewOrderURL);

            return result;
        }

        public override bool UpdateFromClient(ClientData data)
        {
            bool result = base.UpdateFromClient(data);

            Enabled = data.UpdateProperty("enabled", Enabled, ref result);
			Imported = data.UpdateProperty("imported", Imported, ref result);
			Note = data.UpdateProperty("note", Note, ref result);
            ExpiresOn = data.UpdateProperty("expiresOn", ExpiresOn, ref result);
            OwnerId = data.UpdateProperty("ownerId", OwnerId, ref result);
            Product = data.UpdateProperty<ViewOrderProduct>("product", Product, ref result);
            Options = data.UpdateProperty<ViewOrderOptions>("options", Options, ref result);
            MlsId = data.UpdateProperty("mlsId", MlsId, ref result);
            InfoUrl = data.UpdateProperty("infoUrl", InfoUrl, ref result);
            VTourUrl = data.UpdateProperty("vTourUrl", VTourUrl, ref result);

            return result;
        }
    }
}