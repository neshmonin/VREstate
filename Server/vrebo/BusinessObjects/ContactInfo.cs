using System;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public partial class ContactInfo : IClientDataProvider
    {
        public const char ArraySplitterC = ',';
        public const string ArraySplitterS = ",";

        public enum Title
        {
            Mr,
            Mrs,
            Ms,
            Dr
        }

        private int AutoID { get; set; }
        private byte[] Version { get; set; }

        private string Emails { get; set; }
        private string PhoneNumbers { get; set; }

        public Title PersonalTitle { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public string PhotoUriRelative { get; set; }

        public ContactInfo()
        {
            Emails = string.Empty;
            PhoneNumbers = string.Empty;
        }

        //public string[] EmailAddressList
        //{
        //    get
        //    {
        //        if (!string.IsNullOrEmpty(Emails)) return Emails.Split(';');
        //        else return new string[0];
        //    }
        //    set
        //    {
        //        if (null == value) Emails = string.Empty;
        //        else if (0 == value.Length) Emails = string.Empty;
        //        else Emails = string.Join(";", value);
        //    }
        //}

        //public PhoneNumber[] PhoneNumberList
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(PhoneNumbers)) return new PhoneNumber[0];
        //        else
        //        {
        //            string[] phs = PhoneNumbers.Split(';');
        //            PhoneNumber[] result = new PhoneNumber[phs.Length];
        //            for (int idx = phs.Length - 1; idx >= 0; idx--)
        //                result[idx] = new PhoneNumber(phs[idx]);
        //            return result;
        //        }
        //    }
        //    set
        //    {
        //        if (null == value) PhoneNumbers = string.Empty;
        //        else if (0 == value.Length) PhoneNumbers = string.Empty;
        //        else
        //        {
        //            string[] phs = new string[value.Length];
        //            for (int idx = value.Length - 1; idx >= 0; idx--)
        //                phs[idx] = value[idx].ToString();
        //            PhoneNumbers = string.Join(";", phs);
        //        }
        //    }
        //}

        public ClientData GetClientData()
        {
            ClientData result = new ClientData();

            result.Add("personalTitle", PersonalTitle);
            result.Add("firstName", FirstName);
            result.Add("lastName", LastName);
            result.Add("middleName", MiddleName);

            result.Add("addressLine1", AddressLine1);
            result.Add("addressLine2", AddressLine2);
            result.Add("city", City);
            result.Add("stateProvince", StateProvince);
            result.Add("postalCode", PostalCode);
            result.Add("country", Country);

            result.Add("phoneNumbers", PhoneNumbers.Split(ArraySplitterC));
            result.Add("emails", Emails.Split(ArraySplitterC));

            return result;
        }

        public bool UpdateFromClient(ClientData data)
        {
            bool changed = false;

            PersonalTitle = data.UpdateProperty<Title>("personalTitle", PersonalTitle, ref changed);
            FirstName = data.UpdateProperty("firstName", FirstName, ref changed);
            LastName = data.UpdateProperty("lastName", LastName, ref changed);
            MiddleName = data.UpdateProperty("middleName", MiddleName, ref changed);

            AddressLine1 = data.UpdateProperty("addressLine1", AddressLine1, ref changed);
            AddressLine2 = data.UpdateProperty("addressLine2", AddressLine2, ref changed);
            City = data.UpdateProperty("city", City, ref changed);
            StateProvince = data.UpdateProperty("stateProvince", StateProvince, ref changed);
            PostalCode = data.UpdateProperty("postalCode", PostalCode, ref changed);
            Country = data.UpdateProperty("country", Country, ref changed);

            PhoneNumbers = string.Join(ArraySplitterS,
                data.UpdateProperty("phoneNumbers", PhoneNumbers.Split(ArraySplitterC), ref changed));
            Emails = string.Join(ArraySplitterS,
                data.UpdateProperty("emails", Emails.Split(ArraySplitterC), ref changed));

            return changed;
        }
    }
}
