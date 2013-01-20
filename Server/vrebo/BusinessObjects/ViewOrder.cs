﻿using System;

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
        public string Note { get; set; }

        public int RequestCounter { get; private set; }
        public DateTime LastRequestTime { get; private set; }

        public ViewOrderProduct Product { get; private set; }
        public ViewOrderOptions Options { get; private set; }
        public string MlsId { get; private set; }
        public string MlsUrl { get; set; }
        public string ProductUrl { get; private set; }
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
            ProductUrl = productUrl;
            TargetObjectType = type;
            TargetObjectId = targetObjectId;
            ExpiresOn = expiresOn;
            Enabled = true;

            RequestCounter = 0;
            LastRequestTime = DateTime.UtcNow;
        }

        public void Update(
            ViewOrder.ViewOrderOptions options, string mlsId, string mlsUrl,
            string productUrl, DateTime expiresOn)
        {
            Options = options;
            MlsId = mlsId;
            MlsUrl = mlsUrl;
            ProductUrl = productUrl;
            ExpiresOn = expiresOn;

            MarkUpdated();
        }

        public void Touch()
        {
            RequestCounter++;
            LastRequestTime = DateTime.UtcNow;
        }

        public ViewOrder(ClientData data) : base(data)
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
            result.Add("note", Note);

            result.Add("product", ClientData.ConvertProperty<ViewOrderProduct>(Product));
            result.Add("options", ClientData.ConvertProperty<ViewOrderOptions>(Options));
            result.Add("mlsId", MlsId);
            result.Add("mlsUrl", MlsUrl);
            result.Add("productUrl", ProductUrl);
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
            Note = data.GetProperty("note", string.Empty);
            ExpiresOn = data.UpdateProperty("expiresOn", ExpiresOn, ref result);
            OwnerId = data.UpdateProperty("ownerId", OwnerId, ref result);
            Product = data.UpdateProperty<ViewOrderProduct>("product", Product, ref result);
            Options = data.UpdateProperty<ViewOrderOptions>("options", Options, ref result);
            MlsId = data.UpdateProperty("mlsId", MlsId, ref result);
            MlsUrl = data.UpdateProperty("mlsUrl", MlsUrl, ref result);
            ProductUrl = data.UpdateProperty("productUrl", ProductUrl, ref result);

            return result;
        }
    }
}