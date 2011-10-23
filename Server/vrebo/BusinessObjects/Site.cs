using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public partial class Site : UpdateableBase, IClientDataProvider
    {
        public virtual EstateDeveloper Developer { get; protected set; }

        public virtual IList<Building> Buildings { get; protected set; }
        public virtual IList<SuiteType> SuiteTypes { get; protected set; }

        public virtual string Name { get; set; }

        public virtual string GenericInfoModel { get; set; }
        public virtual string ExcursionModel { get; set; }
        public virtual GeoPoint Location { get; set; }

        protected Site() { }

        public Site(EstateDeveloper developer, string name)
        {
            InitializeNew();
            Name = name;
            Developer = developer;
            Location = GeoPoint.Empty;
        }

        public virtual ClientData GetClientData()
        {
            ClientData result = new ClientData();

            result.Add("name", Name);

            //ClientData position = new ClientData();
            //position.Add("lon", Longitude);
            //position.Add("lat", Latitude);
            //position.Add("alt", Altitude);
            //result.Add("position", position);

            result.Add("genericInfoModel", GenericInfoModel);
            result.Add("excursionModel", ExcursionModel);

            return result;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
