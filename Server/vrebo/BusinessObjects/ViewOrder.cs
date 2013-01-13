﻿using System;

namespace Vre.Server.BusinessLogic
{
    public class ViewOrder : UpdateableGuidBase
    {
        public enum ViewOrderType : int
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
        
        public ViewOrderType Product { get; private set; }
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
            ViewOrder.ViewOrderType product, string mlsId, 
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
            ViewOrder.ViewOrderType product, string mlsId,
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

            result.Add("product", ClientData.ConvertProperty<ViewOrderType>(Product));
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
            Product = data.GetProperty<ViewOrderType>("product", ViewOrderType.FloorPlan);
            MlsId = data.GetProperty("mlsIs", string.Empty);
            MlsUrl = data.GetProperty("mlsUrl", string.Empty);
            ProductUrl = data.GetProperty("productUrl", string.Empty);

            return result;
        }
    }
}