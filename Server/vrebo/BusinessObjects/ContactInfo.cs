using System;
using System.Diagnostics;

namespace Vre.Server.BusinessLogic
{
	[Serializable]
    public partial class ContactInfo : IClientDataProvider
	{

        public class PhoneNumber
        {
            public enum Category
            {
                Home,
                Work,
                Mobile,
                Other
            }

            public Category PhoneCategory { get; private set; }
            public string Number { get; private set; }
            public string Extension { get; private set; }

            public PhoneNumber(Category phoneCategory, string number, string extension)
            {
                // TODO: sanitize Number and Extension to numerics only!
                PhoneCategory = phoneCategory;
                Number = number;
                Extension = extension;
            }

            public PhoneNumber(string encoded)
            {
                Debug.Assert((encoded != null) && (encoded.Length > 2), "Phone number format is invalid!");
                
                char type = encoded[0];
                if ('h' == type) PhoneCategory = Category.Home;
                else if ('m' == type) PhoneCategory = Category.Mobile;
                else if ('w' == type) PhoneCategory = Category.Work;
                else PhoneCategory = Category.Other;

                int pos = encoded.IndexOf('#');
                Number = encoded.Substring(2, pos - 2);
                Extension = encoded.Substring(pos + 1);
            }

            public override string ToString()
            {
                char type;
                switch (PhoneCategory)
                {
                    case Category.Home: type = 'h'; break;
                    case Category.Mobile: type = 'm'; break;
                    case Category.Work: type = 'w'; break;
                    default: type = 'o'; break;
                }
                return string.Format("{0};{1}#{2}", type, Number, Extension);
            }
        }

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

        public ContactInfo()
		{
            Emails = string.Empty;
            PhoneNumbers = string.Empty;
		}

        public string[] EmailAddressList
        {
            get
            {
                if (!string.IsNullOrEmpty(Emails)) return Emails.Split(';');
                else return new string[0];
            }
            set
            {
                if (null == value) Emails = string.Empty;
                else if (0 == value.Length) Emails = string.Empty;
                else Emails = string.Join(";", value);
            }
        }

        public PhoneNumber[] PhoneNumberList
        {
            get
            {
                if (string.IsNullOrEmpty(PhoneNumbers)) return new PhoneNumber[0];
                else
                {
                    string[] phs = PhoneNumbers.Split(';');
                    PhoneNumber[] result = new PhoneNumber[phs.Length];
                    for (int idx = phs.Length - 1; idx >= 0; idx--)
                        result[idx] = new PhoneNumber(phs[idx]);
                    return result;
                }
            }
            set
            {
                if (null == value) PhoneNumbers = string.Empty;
                else if (0 == value.Length) PhoneNumbers = string.Empty;
                else
                {
                    string[] phs = new string[value.Length];
                    for (int idx = value.Length - 1; idx >= 0; idx--)
                        phs[idx] = value[idx].ToString();
                    PhoneNumbers = string.Join(";", phs);
                }
            }
        }

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

            result.Add("phoneNumbers", PhoneNumbers);
            result.Add("emails", Emails);

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

            PhoneNumbers = data.UpdateProperty("phoneNumbers", PhoneNumbers, ref changed);
            Emails = data.UpdateProperty("emails", Emails, ref changed);

            return changed;
        }
    }
}
