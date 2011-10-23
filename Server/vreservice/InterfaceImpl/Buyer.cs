using System.Collections.Generic;
using System;

namespace Vre.Server.BusinessLogic
{
    public class BuyerInterface : UserInterface, IBuyer
    {
        internal override bool IsAllowed(User.Role role)
        {
            // ROLE PERMISSION LOGIC
            if (
                (role == User.Role.Buyer) ||
                (role == User.Role.DeveloperAdmin) ||
                (role == User.Role.SuperAdmin)
                ) return true;
            return false;
        }

        public List<Account> GetAccounts()
        {
            if (!IsAllowed()) return null;
            setupContext(false);

            List<Account> result = new List<Account>();
            result.Add(new Account(0, 0));
            return result;

            //throw new NotImplementedException();
        }

        public ContactInfo GetContactInfo()
        {
            if (!IsAllowed()) return null;
            setupContext(false);

            throw new NotImplementedException();
        }
    }
}