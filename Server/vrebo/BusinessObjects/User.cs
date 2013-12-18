using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vre.Server.BusinessLogic
{
    // About object locking: http://ayende.com/Blog/archive/2009/04/15/nhibernate-mapping-concurrency.aspx

	[Serializable]
	public partial class User : UpdateableBase
	{
	    public enum Role : byte
	    {
			/// <summary>
			/// Usual property buyer
			/// </summary>
            Buyer = 0,
			/// <summary>
			/// Superadministrator with full system access
			/// </summary>
            SuperAdmin = 1,
			/// <summary>
			/// Property developer administrator with most rights applied to owning EstateDeveloper object
			/// </summary>
            DeveloperAdmin = 2,
			/// <summary>
			/// Subcontractor
			/// </summary>
            Subcontractor = 3,
			/// <summary>
			/// Sales person 
			/// </summary>
            SalesPerson = 4,
			/// <summary>
			/// Independent selling agent
			/// </summary>
            SellingAgent = 5,
			/// <summary>
			/// Visitor having no account in system; independent and with minimal rights
			/// </summary>
            Visitor = 6,
			/// <summary>
			/// Kiosk installed on property developer's premises
			/// </summary>
            Kiosk = 7,
			/// <summary>
			/// Independent buying agent
			/// </summary>
			BuyingAgent = 8,
			/// <summary>
			/// Anonymous account created automatically; has no related login!
			/// </summary>
			Anonymous = 9
	    }

        public Role UserRole { get; private set; }
        public int? EstateDeveloperID { get; private set; }
        public DateTime LastLogin { get; set; }
		[StringLength(256)]
		public string NickName { get; set; }
		[StringLength(256)]
		public string PrimaryEmailAddress { get; set; }
        /// <summary>
        /// For handling: http://code.google.com/p/noda-time/
        /// </summary>
		[StringLength(256)]
		public string TimeZone { get; set; }
		public string PersonalInfo { get; set; }
        //public ContactInfo PersonalInfo { get; private set; }
        public BrokerageInfo BrokerInfo { get; private set; }
        public IList<UserLicense> Licenses { get; private set; }
        public decimal CreditUnits { get; private set; }

		public string[] RefererRestriction 
		{
			get { return string.IsNullOrEmpty(refererRestriction) ? new string[0] : refererRestriction.Split('\0'); }
			set { refererRestriction = ((value != null) && (value.Length > 0)) ? value.ToString('\0') : null; }
		}
		private string refererRestriction { get; set; }

        /// <summary>
        /// Suites managed by this user (sales person or selling agent); initially only used by latter.
        /// </summary>
        public virtual ICollection<Suite> ManagedSuites { get; private set; }
        /// <summary>
        /// Buildings managed by this user (sales person or selling agent); initially only used by latter.
        /// </summary>
        public virtual ICollection<Building> ManagedBuildings { get; private set; }

        /// <summary>
        /// List of other users (sellers) which can see this user's information
        /// </summary>
        public virtual ICollection<User> VisibleBy { get; protected set; }
        /// <summary>
        /// List of users (buyers) this user (seller) can view
        /// </summary>
        public virtual ICollection<User> CanView { get; protected set; }

        private User() { }
        
		public User(int? estateDeveloperId, Role role) : base()
		{
            InitializeNew();
            UserRole = role;
            if (role == Role.Anonymous)
                AutoID = 64;
            EstateDeveloperID = estateDeveloperId;
            NickName = string.Empty;
            PrimaryEmailAddress = string.Empty;
            TimeZone = string.Empty;
            PersonalInfo = null;// new ContactInfo();
            BrokerInfo = null;// new BrokerageInfo();
            ManagedSuites = new List<Suite>();
            ManagedBuildings = new List<Building>();
            VisibleBy = new HashSet<User>();
            CanView = new HashSet<User>();
            Licenses = new List<UserLicense>();
            CreditUnits = 0m;
			RefererRestriction = null;
			LastLogin = new DateTime(1900, 01, 01);
        }

        /// <summary>
        /// True if this role requires valid Estate Developer association.
        /// If false, NO Estate Developer must be associated with this user.
        /// </summary>
        public static bool IsEstateDeveloperTied(Role role)
        {
            return ((role != Role.SellingAgent) && (role != Role.BuyingAgent) 
				&& (role != Role.SuperAdmin) && (role != Role.Buyer) && (role != Role.Visitor)
				&& (role != Role.Anonymous));
        }

        public void UpdatePersonalInfo(string info)
        {
            PersonalInfo = info;
            MarkUpdated();
        }

        public UserLicense EmitLicense(Site constructionSite, DateTime expiryTime, User initiator)
        {
            UserLicense result = null;
            foreach (UserLicense ul in Licenses)
                if (ul.LicensedSite.Equals(constructionSite))
                {
                    result = ul;
                    result.Prolong(expiryTime, initiator);
                    break;
                }

            if (null == result)
            {
                result = new UserLicense(this, constructionSite, expiryTime, initiator);
                Licenses.Add(result);
            }

            MarkUpdated();

            return result;
        }

        /// <summary>
        /// Returns true if user has a valid license for this site.
        /// </summary>
        public bool HasLicense(Site constructionSite)
        {
            bool result = false;

            foreach (UserLicense ul in Licenses)
                if (ul.LicensedSite.Equals(constructionSite))
                {
                    result = (ul.ExpiryTime > DateTime.UtcNow);
                    break;
                }

            return result;
        }

        /// <summary>
        /// Returns true if user has any valid license.
        /// </summary>
        public bool HasAnyLicense()
        {
            bool result = false;

            foreach (UserLicense ul in Licenses)
                if (ul.ExpiryTime > DateTime.UtcNow)
                {
                    result = true;
                    break;
                }

            return result;
        }

        public void Touch()
        {
            LastLogin = DateTime.UtcNow;
            MarkUpdated();
        }

        public User(ClientData data) : base(data)
        {
            EstateDeveloperID = data.GetProperty("estateDeveloperId", -1);
            UserRole = data.GetProperty<User.Role>("role", Role.Visitor);
            NickName = data.GetProperty("nickName", string.Empty);
            // TODO: deal with personal info (vCard) and brokerage
        }

        public override ClientData GetClientData()
        {
            ClientData result = base.GetClientData();

            result.Add("estateDeveloperId", EstateDeveloperID);  // informational only
            result.Add("role", ClientData.ConvertProperty<Role>(UserRole));  // informational only

            result.Add("nickName", NickName);
            result.Add("primaryEmail", PrimaryEmailAddress);
            result.Add("timeZone", TimeZone);
            if (PersonalInfo != null)
                result.Add("personalInfo", PersonalInfo);

            if (BrokerInfo != null)
                result.Add("brokerageInfo", BrokerInfo.GetClientData());

			if (refererRestriction != null)
				result.Add("refererRestriction", RefererRestriction);

            return result;
        }

        public override bool UpdateFromClient(ClientData data)
        {
            bool changed = base.UpdateFromClient(data);

            NickName = data.UpdateProperty("nickName", NickName, ref changed);
            PrimaryEmailAddress = data.UpdateProperty("primaryEmail", PrimaryEmailAddress, ref changed);
            TimeZone = data.UpdateProperty("timeZone", TimeZone, ref changed);
            PersonalInfo = data.UpdateProperty("personalInfo", PersonalInfo, ref changed);
			RefererRestriction = data.UpdateProperty("refererRestriction", RefererRestriction, ref changed);

            ClientData bicd = data.GetNextLevelDataItem("brokerageInfo");
            if (bicd.Count > 0)
            {
                if (null == BrokerInfo) BrokerInfo = new BrokerageInfo();
                if (BrokerInfo.UpdateFromClient(bicd)) changed = true;
            }

            return changed;
        }

        public override string ToString()
        {
            if (EstateDeveloperID != null)
                return string.Format("ID={0},r={1},ED={2}", AutoID, UserRole, EstateDeveloperID);
            else
                return string.Format("ID={0},r={1},e={2}", AutoID, UserRole, PrimaryEmailAddress);
        }
    }
}
