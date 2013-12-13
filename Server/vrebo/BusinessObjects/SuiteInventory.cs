using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public partial class SuiteInventory : UpdateableBase
    {
        public virtual string SuiteName { get; set; }
        public virtual int PhysicalLevelNumber { get; set; }
        public virtual string FloorName { get; set; }
        public virtual Suite.SalesStatus Status { get; set; }
        public virtual Money? CurrentPrice { get; set; }
        public virtual string FloorplanURL { get; set; }
        public virtual double Area { get; set; }
        public virtual int Bedrooms { get; set; }
        public virtual int Dens { get; set; }
        public virtual double Bathrooms { get; set; }
        public virtual int Balconies { get; set; }
        public virtual int Terraces { get; set; }
        public virtual string VirtualTourURL { get; set; }
        public virtual string MoreInfoURL { get; set; }
        public virtual string MLS { get; set; }
        public virtual string Address { get; set; }

        public SuiteInventory() {}

        public SuiteInventory(SuiteInventory copy)
            : base(copy)
        {
            SuiteName = copy.SuiteName;
            PhysicalLevelNumber = copy.PhysicalLevelNumber;
            FloorName = copy.FloorName;
            Status = copy.Status;

            FloorplanURL = copy.FloorplanURL;
            Area = copy.Area;
            Bedrooms = copy.Bedrooms;
            Dens = copy.Dens;
            Bathrooms = copy.Bathrooms;
            Balconies = copy.Balconies;
            Terraces = copy.Terraces;
            VirtualTourURL = copy.VirtualTourURL;
            MoreInfoURL = copy.MoreInfoURL;
            MLS = copy.MLS;
            Address = copy.Address;
            CurrentPrice = copy.CurrentPrice;
        }

        public SuiteInventory(ClientData fromServer)
        {
            InitializeNew();
            AutoID = fromServer.GetProperty("id", -1);

            bool changed = UpdateFromClient(fromServer);

            FloorName = fromServer.UpdateProperty("floorName", FloorName, ref changed);

            decimal cp = fromServer.GetProperty("currentPrice", -1.0m);
	        Currency cpc;
			if ((cp >= 0.0m) && Currency.TryParse(fromServer.GetProperty("currentPriceCurrency", string.Empty), out cpc))
				CurrentPrice = new Money(cp, cpc);
        }

        public override ClientData GetClientData()
        {
            ClientData result = base.GetClientData();

            result.Add("levelNumber", PhysicalLevelNumber);
            result.Add("floorName", FloorName);
            result.Add("name", SuiteName);
            result.Add("status", ClientData.ConvertProperty<Suite.SalesStatus>(Status));

            if (CurrentPrice.HasValue)
            {
                result.Add("currentPrice", CurrentPrice.Value.ToString("F"));
                result.Add("currentPriceDisplay", CurrentPrice.Value.ToString("C"));
                result.Add("currentPriceCurrency", CurrentPrice.Value.Currency.Iso3LetterCode);
            }

            return result;
        }

        public override bool UpdateFromClient(ClientData data)
        {
            bool changed = base.UpdateFromClient(data);

            PhysicalLevelNumber = data.UpdateProperty("levelNumber", PhysicalLevelNumber, ref changed);
            SuiteName = data.UpdateProperty("name", SuiteName, ref changed);
            FloorName = data.UpdateProperty("floorName", FloorName, ref changed);
            Status = data.UpdateProperty("status", Status, ref changed);
            if (CurrentPrice.HasValue)
            {
                CurrentPrice = new Money(
                    data.UpdateProperty("currentPrice", Convert.ToDecimal(CurrentPrice.Value), ref changed),
                    CurrentPrice.Value.Currency);
            }
            else
            {
                var value = data.GetProperty("currentPrice", -1.0m);
                if (value >= 0.0m)
                {
                    Currency c;
                    if (!Currency.TryParse(data.GetProperty("currentPriceCurrency", "CAD"), out c))
                        c = Currency.Cad;

                    CurrentPrice = new Money(value, c);
                    changed = true;
                }
            }


            VirtualTourURL = data.UpdateProperty("vTourUrl", VirtualTourURL, ref changed);
            MoreInfoURL = data.UpdateProperty("infoUrl", MoreInfoURL, ref changed);
            MLS = data.UpdateProperty("mlsId", MLS, ref changed);
            Address = data.UpdateProperty("address", Address, ref changed);

            FloorplanURL = data.UpdateProperty("floorPlanUrl", FloorplanURL, ref changed);
            Area = data.UpdateProperty("area", Area, ref changed);
            Bedrooms = data.UpdateProperty("bedrooms", Bedrooms, ref changed);
            Dens = data.UpdateProperty("dens", Dens, ref changed);
            Bathrooms = data.UpdateProperty("bathrooms", Bathrooms, ref changed);
            Balconies = data.UpdateProperty("balconies", Balconies, ref changed);
            Terraces = data.UpdateProperty("terraces", Terraces, ref changed);

            return changed;
        }

        public override string ToString()
        {
            return SuiteName;
        }
    }
}
