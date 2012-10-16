using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public partial class Site : UpdateableBase
    {
        public virtual EstateDeveloper Developer { get; protected set; }

        public virtual IList<Building> Buildings { get; protected set; }
        public virtual IList<SuiteType> SuiteTypes { get; protected set; }

        public virtual string Name { get; set; }

        public virtual string DisplayModelUrl { get; set; }
        public virtual string OverlayModelUrl { get; set; }
        public virtual string BubbleTemplateUrl { get; set; }
        public virtual string ExcursionModel { get; set; }
        public virtual GeoPoint Location { get; set; }
        public virtual string InitialView { get; set; }
        public virtual string WireframeLocation { get; set; }

        protected Site() { }

        public Site(EstateDeveloper developer, string name) : base()
        {
            InitializeNew();
            Name = name;
            Developer = developer;
            Location = GeoPoint.Empty;
            Buildings = new List<Building>();
            SuiteTypes = new List<SuiteType>();
            if (developer != null) developer.Sites.Add(this);
            InitialView = string.Empty;
        }

        public virtual ClientData GetClientData()
        {
            ClientData result = base.GetClientData();

            result.Add("estateDeveloperId", Developer.AutoID);
            result.Add("name", Name);

            //ClientData position = new ClientData();
            //position.Add("lon", Longitude);
            //position.Add("lat", Latitude);
            //position.Add("alt", Altitude);
            //result.Add("position", position);

            if (!string.IsNullOrEmpty(DisplayModelUrl))
                result.Add("displayModelUrl", DisplayModelUrl);
            if (!string.IsNullOrEmpty(OverlayModelUrl))
                result.Add("overlayModelUrl", OverlayModelUrl);
            if (!string.IsNullOrEmpty(BubbleTemplateUrl))
                result.Add("bubbleTemplateUrl", BubbleTemplateUrl);
            if (!string.IsNullOrEmpty(ExcursionModel))
                result.Add("excursionModel", ExcursionModel);

            result.Add("initialView", InitialView);

            return result;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
