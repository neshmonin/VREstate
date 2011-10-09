using System.Collections.Generic;
using System;

namespace Vre.Server.BusinessLogic
{
    public class SalespersonInterface : UserInterface, ISalesperson
    {
        internal override bool IsAllowed(User.Role role)
        {
            if (
                // ROLE PERMISSION LOGIC
                (role == User.Role.SalesPerson) ||
                (role == User.Role.DeveloperAdmin) ||
                (role == User.Role.SuperAdmin)
                ) return true;
            return false;
        }


    }
}