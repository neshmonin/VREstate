using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public partial class SuiteType : UpdateableBase
    {
        public virtual Site ConstructionSite { get; protected set; }
        public virtual string Name { get; set; }
        public virtual string Model { get; set; }
        public virtual IList<SuiteLevel> Levels { get; protected set; }

        public virtual ValueWithUM FloorArea { get; set; }  // TODO: Should be computable field!
        public virtual int BedroomCount { get; set; }  // TODO: Should be computable field!
        public virtual int DenCount { get; set; }  // TODO: Should be computable field!
        public virtual int BathroomCount { get; set; }  // TODO: Should be computable field!
        public virtual int BalconyCount { get; set; }  // TODO: Should be computable field!
        public virtual int TerraceCount { get; set; }  // TODO: Should be computable field!

        /// <summary>
        /// Not used; required for proper inverse mapping and updating references from suites
        /// </summary>
        protected virtual IList<Suite> SuitesInvolved { get; set; }

        protected virtual string floorArea { get { return FloorArea.AsRaw; } set { FloorArea = new ValueWithUM(value); } }

        public virtual Wireframe[] WireframeModel { get; set; }

        protected SuiteType() { }
        public SuiteType(Site constructionSite, string name) : base()
        {
            InitializeNew();
            ConstructionSite = constructionSite;
            Name = name;
            Model = null;
            Levels = new List<SuiteLevel>();
            WireframeModel = null;
            SuitesInvolved = new List<Suite>();
            if (constructionSite != null) constructionSite.SuiteTypes.Add(this);
        }

        public override ClientData GetClientData()
        {
            ClientData result = new ClientData();
            int idx;

            result.Add("id", AutoID);  // informational only

            result.Add("name", Name);
            result.Add("modelName", Model);  // informatinal only

            if (Levels != null)
            {
                idx = 0;
                ClientData[] levels = new ClientData[Levels.Count];
                foreach (SuiteLevel sl in Levels) levels[idx++] = sl.GetClientData();
                result.Add("levels", levels);
            }

            if (WireframeModel != null)
            {
                idx = 0;
                ClientData[] model = new ClientData[WireframeModel.Length];
                foreach (Wireframe wf in WireframeModel) model[idx++] = wf.GetClientData();
                result.Add("geometries", model);  // keep model's terminology
            }

            if (FloorArea != null)
            {
                switch (FloorArea.StoredAs)
                {
                    case ValueWithUM.Unit.SqFeet:
                        result.Add("area", FloorArea.ValueAs(FloorArea.StoredAs));
                        result.Add("areaUm", "sqFt");
                        break;

                    case ValueWithUM.Unit.SqMeters:
                        result.Add("area", FloorArea.ValueAs(FloorArea.StoredAs));
                        result.Add("areaUm", "sqM");
                        break;
                }
            }

            result.Add("bedrooms", BedroomCount);
            result.Add("dens", DenCount);
            result.Add("bathrooms", BathroomCount);
            result.Add("balconies", BalconyCount);
            result.Add("terraces", TerraceCount);

            return result;
        }
    }
}
