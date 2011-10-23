using System.Collections.Generic;
using System;

namespace Vre.Server.BusinessLogic
{
    public class SubcontractorInterface : UserInterface, ISubcontractor
    {
        internal override bool IsAllowed(User.Role role)
        {
            if (
                // ROLE PERMISSION LOGIC
                (role == User.Role.Subcontractor) ||
                (role == User.Role.DeveloperAdmin) ||
                (role == User.Role.SuperAdmin)
                ) return true;
            return false;
        }


        public IList<Option> ListOptionsByBuilding(int buildingId)
        {
            if (null == _user) return null;

            throw new NotImplementedException();
        }

        public IList<Option> ListOptionsByBuildingAndSuite(int buildingId, int suiteId)
        {
            if (null == _user) return null;

            throw new NotImplementedException();
        }

        public void UpdateCutOffDate(int optionId, int buildingId, DateTime cutOffDate)
        {
            if (null == _user) return;

            throw new NotImplementedException();
        }
    }
}