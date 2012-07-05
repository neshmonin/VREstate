using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vre.Server.BusinessLogic
{
    public class UserLicense : UpdateableBase
    {
        public User Licensee { get; private set; }
        public Site LicensedSite { get; private set; }
        public DateTime ExpiryTime { get; private set; }
        public User LastUpdatedBy { get; private set; }

        private UserLicense() { }

        public UserLicense(User licensee, Site licensedSite, DateTime expiryTime, User initiator)
        {
            InitializeNew();

            Licensee = licensee;
            LicensedSite = licensedSite;
            ExpiryTime = expiryTime;
            LastUpdatedBy = initiator;
        }

        public void Prolong(DateTime newExpiryTime, User modifier)
        {
            ExpiryTime = newExpiryTime;
            LastUpdatedBy = modifier;
            MarkUpdated();
        }
    }
}
