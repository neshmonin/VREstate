using System;
using System.Diagnostics;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public partial class BrokerageInfo : UpdateableBase//IClientDataProvider
    {
		private string Emails { get; set; }
		private string PhoneNumbers { get; set; }
        
        public string Name { get; set; }

        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public string WebSite { get; set; }

        public string LogoUriRelative { get; set; }

		public decimal CreditUnits { get; private set; }
		public DateTime LastServicePayment { get; set; }

		public string[] EmailList
        {
            get
            {
                if (string.IsNullOrEmpty(Emails))
                    return null;
                return Emails.Split(ContactInfo.ArraySplitterC);
            }
        }
		public string[] PhoneNumberList
        {
            get
            {
                if (string.IsNullOrEmpty(PhoneNumbers))
                    return null;
                return PhoneNumbers.Split(ContactInfo.ArraySplitterC);
            }
        }

		private BrokerageInfo() { }

		public BrokerageInfo(string name)
        {
			InitializeNew();
			Name = name;
			Emails = string.Empty;
			PhoneNumbers = string.Empty;
			CreditUnits = 0m;
			LastServicePayment = new DateTime(1900, 01, 01);
        }

		public BrokerageInfo(ClientData data)
            : base(data)
        {
			UpdateFromClient(data);
        }

        public override ClientData GetClientData()
        {
			ClientData result = base.GetClientData();

			result.Add("name", Name);

            result.Add("streetAddress", StreetAddress);
            result.Add("city", City);
            result.Add("stateProvince", StateProvince);
            result.Add("postalCode", PostalCode);
            result.Add("country", Country);

			result.Add("phoneNumbers", PhoneNumbers.Split(ContactInfo.ArraySplitterC));
			result.Add("emails", Emails.Split(ContactInfo.ArraySplitterC));

            result.Add("webSite", WebSite);

            return result;
        }

        public bool UpdateFromClient(ClientData data)
        {
            bool changed = false;

			Name = data.UpdateProperty("name", Name, ref changed);

            StreetAddress = data.UpdateProperty("streetAddress", StreetAddress, ref changed);
            City = data.UpdateProperty("city", City, ref changed);
            StateProvince = data.UpdateProperty("stateProvince", StateProvince, ref changed);
            PostalCode = data.UpdateProperty("postalCode", PostalCode, ref changed);
            Country = data.UpdateProperty("country", Country, ref changed);

            if (string.IsNullOrEmpty(PhoneNumbers))
                data.UpdateProperty("phoneNumbers", new string[0], ref changed);
            else
                PhoneNumbers = string.Join(ContactInfo.ArraySplitterS, 
                    data.UpdateProperty("phoneNumbers", PhoneNumbers.Split(ContactInfo.ArraySplitterC), ref changed));

            if (string.IsNullOrEmpty(Emails))
                data.UpdateProperty("emails", new string[0], ref changed);
            else
                Emails = string.Join(ContactInfo.ArraySplitterS,
                    data.UpdateProperty("emails", Emails.Split(ContactInfo.ArraySplitterC), ref changed));

			WebSite = data.UpdateProperty("webSite", WebSite, ref changed);

            return changed;
        }

		public override string ToString()
		{
			return Name;
		}

        public void Debit(decimal units)
        {
            CreditUnits -= units;
            MarkUpdated();
        }

        public void Credit(decimal units)
        {
            CreditUnits += units;
            MarkUpdated();
        }
    }
}
