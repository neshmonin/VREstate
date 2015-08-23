using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public partial class SuiteLevel : UpdateableBase
    {
        public virtual SuiteType SuiteType { get; protected set; }
        public virtual int Order { get; set; }
        public virtual string Name { get; set; }
		public virtual string LocalizedName { get; set; }
		public virtual string Model { get; set; }
        public virtual IList<Room> Rooms { get; protected set; }

        #region consolidated searchable information
        public virtual int Bedrooms { get; set; }
        public virtual int Dens { get; set; }
        public virtual int Toilets { get; set; }
        public virtual int Showers { get; set; }
        public virtual int Baths { get; set; }
        public virtual int Balconies { get; set; }
        #endregion

        protected SuiteLevel() { }
        public SuiteLevel(SuiteType suiteType, string name) : base()
        {
            InitializeNew();
            SuiteType = suiteType;
            Name = name;
			LocalizedName = name;
            Order = 0;
            Model = null;
            Rooms = new List<Room>();
        }

        public override ClientData GetClientData()
        {
            ClientData result = new ClientData();

            result.Add("id", AutoID);  // informational only

            result.Add("order", Order);
            result.Add("name", LocalizedName);
            result.Add("modelName", Model);  // informatinal only

            // TODO: Rooms!

            result.Add("bedrooms", Bedrooms);
            result.Add("dens", Dens);
            result.Add("toilets", Toilets);
            result.Add("showers", Showers);
            result.Add("baths", Baths);
            result.Add("balconies", Balconies);

            return result;
        }
    }
}
