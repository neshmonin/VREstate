using System;
using System.Diagnostics;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public partial class BrokerageInfo : IClientDataProvider
    {

        private int AutoID { get; set; }
        private byte[] Version { get; set; }

        private string Emails { get; set; }
        private string PhoneNumbers { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public string WebSite { get; set; }

        public string LogoUriRelative { get; set; }

        public BrokerageInfo()
        {
            Emails = string.Empty;
            PhoneNumbers = string.Empty;
        }

        public ClientData GetClientData()
        {
            ClientData result = new ClientData();

            result.Add("addressLine1", AddressLine1);
            result.Add("addressLine2", AddressLine2);
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

            AddressLine1 = data.UpdateProperty("addressLine1", AddressLine1, ref changed);
            AddressLine2 = data.UpdateProperty("addressLine2", AddressLine2, ref changed);
            City = data.UpdateProperty("city", City, ref changed);
            StateProvince = data.UpdateProperty("stateProvince", StateProvince, ref changed);
            PostalCode = data.UpdateProperty("postalCode", PostalCode, ref changed);
            Country = data.UpdateProperty("country", Country, ref changed);

            PhoneNumbers = string.Join(ContactInfo.ArraySplitterS,
                data.UpdateProperty("phoneNumbers", PhoneNumbers.Split(ContactInfo.ArraySplitterC), ref changed));
            Emails = string.Join(ContactInfo.ArraySplitterS,
                data.UpdateProperty("emails", Emails.Split(ContactInfo.ArraySplitterC), ref changed));

            return changed;
        }
    }
}
