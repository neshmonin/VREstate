using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    // About object locking: http://ayende.com/Blog/archive/2009/04/15/nhibernate-mapping-concurrency.aspx

	[Serializable]
	public partial class User : UpdateableBase
	{
	    public enum Role : byte
	    {
            Buyer = 0,
            SuperAdmin = 1,
            DeveloperAdmin = 2,
            Subcontractor = 3,
            SalesPerson = 4,
            SellingAgent = 5,
            Visitor = 6,
            Kiosk = 7
	    }

        public Role UserRole { get; private set; }
        public int? EstateDeveloperID { get; private set; }
        public DateTime LastLogin { get; set; }
        public ContactInfo PersonalInfo { get; private set; }
        public BrokerageInfo BrokerInfo { get; private set; }
        public IList<UserLicense> Licenses { get; private set; }

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
        
		public User(int? estateDeveloperId, Role role)
		{
            InitializeNew();
            UserRole = role;
            EstateDeveloperID = estateDeveloperId;
            PersonalInfo = null;// new ContactInfo();
            BrokerInfo = null;// new BrokerageInfo();
            ManagedSuites = new List<Suite>();
            ManagedBuildings = new List<Building>();
            VisibleBy = new HashSet<User>();
            CanView = new HashSet<User>();
            Licenses = new List<UserLicense>();
        }

        /// <summary>
        /// True if this role requires valid Estate Developer association.
        /// If false, NO Estate Developer must be associated with this user.
        /// </summary>
        public static bool IsEstateDeveloperTied(Role role)
        {
            return ((role != Role.SellingAgent) && (role != Role.SuperAdmin) && (role != Role.Buyer) && (role != Role.Visitor));
        }

        public void UpdatePersonalInfo(ContactInfo info)
        {
            PersonalInfo = info;
            MarkUpdated();
        }

        public void EmitLicense(Site constructionSite, DateTime expiryTime, User initiator)
        {
            bool updated = false;
            foreach (UserLicense ul in Licenses)
                if (ul.LicensedSite.Equals(constructionSite))
                {
                    ul.Prolong(expiryTime, initiator);
                    updated = true;
                }

            if (!updated)
                Licenses.Add(new UserLicense(this, constructionSite, expiryTime, initiator));

            MarkUpdated();
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

        public override ClientData GetClientData()
        {
            ClientData result = new ClientData();

            result.Add("id", AutoID);  // informational only
            result.Add("deleted", Deleted);  // informational only

            result.Add("estateDeveloperId", EstateDeveloperID);  // informational only
            result.Add("role", UserRole);  // informational only

            if (PersonalInfo != null)
                result.Add("personalInfo", PersonalInfo.GetClientData());

            if (BrokerInfo != null)
                result.Add("brokerageInfo", BrokerInfo.GetClientData());

            return result;
        }

        public override bool UpdateFromClient(ClientData data)
        {
            bool changed = false;

            ClientData picd = data.GetNextLevelDataItem("personalInfo");
            if (picd.Count > 0)
            {
                if (null == PersonalInfo) PersonalInfo = new ContactInfo();
                if (PersonalInfo.UpdateFromClient(picd)) changed = true;
            }

            ClientData bicd = data.GetNextLevelDataItem("brokerageInfo");
            if (bicd.Count > 0)
            {
                if (null == BrokerInfo) BrokerInfo = new BrokerageInfo();
                if (BrokerInfo.UpdateFromClient(bicd)) changed = true;
            }

            return changed;
        }
    }
}
