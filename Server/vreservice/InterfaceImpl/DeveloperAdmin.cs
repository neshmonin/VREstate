using System.Collections.Generic;
using System;

namespace Vre.Server.BusinessLogic
{
    public class DeveloperAdminInterface : IDeveloperAdmin
    {
        protected UserInterface _userInterface = new UserInterface();
        protected SubcontractorInterface _subcontractor = new SubcontractorInterface();

        public User[] ListUsers(User.Role role)
        {
            if (null == _userInterface._user) return null;

            throw new NotImplementedException();
        }

        public IList<Option> ListOptions(bool enabledOnly)
        {
            if (null == _userInterface._user) return null;

            throw new NotImplementedException();
        }

        public void CreateOption(Option option)
        {
            if (null == _userInterface._user) return;

            throw new NotImplementedException();
        }

        public void UpdateOption(Option option)
        {
            if (null == _userInterface._user) return;

            throw new NotImplementedException();
        }

        public User GetBuyerDetails(User buyer)
        {
            if (null == _userInterface._user) return null;

            throw new NotImplementedException();
        }

        public void CancelTransaction(Transaction tran)
        {
            if (null == _userInterface._user) return;

            throw new NotImplementedException();
        }

        #region user management
        public bool CreateSubcontractor(LoginType type, string login, string password, out string errorReason)
        {
            return createUser(User.Role.Subcontractor, type, login, password, out errorReason);
        }

        public bool CreateSalesperson(LoginType type, string login, string password, out string errorReason)
        {
            return createUser(User.Role.SalesPerson, type, login, password, out errorReason);
        }

        public bool CreateAdmin(LoginType type, string login, string password, out string errorReason)
        {
            return createUser(User.Role.DeveloperAdmin, type, login, password, out errorReason);
        }

        private bool createUser(User.Role role, LoginType type, string login, string password, out string errorReason)
        {
            if (null == _userInterface._user) { errorReason = "Not logged in."; return false; }
            _userInterface.setupContext(false);

            try
            {
                using (UserManager um = _userInterface.getUserManager())
                {
                    return um.CreateUser(role, _userInterface._user.EstateDeveloperID.Value,
                        type, login, password, out errorReason);
                }
            }
            catch (Exception ex)
            {
                ServiceInstances.Logger.Error("Login create failed (2): {0}.", ex);
                errorReason = "Operation failed (2).  Try again later.";
                return false;
            }
        }

        public bool DeleteUser(User user, out string errorReason)
        {
            if (null == _userInterface._user) { errorReason = "Not logged in."; return false; }
            _userInterface.setupContext(false);

            bool result = false;

            try
            {
                if (user != null)
                {
                    if (user.EstateDeveloperID == _userInterface._user.EstateDeveloperID)
                    {
                        // ROLE PERMISSION LOGIC
                        if ((user.UserRole == User.Role.DeveloperAdmin) ||
                            (user.UserRole == User.Role.SalesPerson) ||
                            (user.UserRole == User.Role.Subcontractor))
                        {
                            using (UserManager um = _userInterface.getUserManager())
                            {
                                result = um.DeleteUser(user, out errorReason);
                            }
                        }
                        else
                        {
                            errorReason = "Cannot delete this user type.";
                        }
                    }
                    else
                    {
                        errorReason = "Cannot delete this user.";
                    }
                }
                else
                {
                    errorReason = "Parameter is null.";
                }
            }
            catch (Exception ex)
            {
                ServiceInstances.Logger.Error("Login delete failed (2): {0}.", ex);
                errorReason = "Operation failed (2).  Try again later.";
            }

            return result;
        }
        #endregion

        public bool Login(LoginType type, string userId, string password, out string errorReason)
        {
            // CAREFUL: This method has no try/catch as all process happens in wrapped object with try/catch logic.

            bool result = _userInterface.Login(type, userId, password, out errorReason);

            if (result)
            {
                // ROLE PERMISSION LOGIC
                if (_userInterface._user.UserRole != User.Role.DeveloperAdmin)
                {
                    ServiceInstances.Logger.Error("Error logging user in: this login is not allowed to use this interface.");
                    errorReason = "This login is not allowed to use this interface.";
                    _userInterface.doLogout();
                    result = false;
                }
            }

            return result;
        }

        #region inherited interface wrappers
        public IList<Option> ListOptionsByBuilding(int buildingId)
        {
            return _subcontractor.ListOptionsByBuilding(buildingId);
        }

        public IList<Option> ListOptionsByBuildingAndSuite(int buildingId, int suiteId)
        {
            return _subcontractor.ListOptionsByBuildingAndSuite(buildingId, suiteId);
        }

        public void UpdateCutOffDate(int optionId, int buildingId, DateTime cutOffDate)
        {
            _subcontractor.UpdateCutOffDate(optionId, buildingId, cutOffDate);
        }

        public bool CreateBuyer(string developerId, LoginType type, string userId, string password, out string errorReason)
        {
            return _userInterface.CreateBuyer(developerId, type, userId, password, out errorReason);
        }

        public ContactInfo GetMyContactInfo()
        {
            return _userInterface.GetMyContactInfo();
        }

        public DataUpdateResult UpdateMyContactInfo(ContactInfo info)
        {
            return _userInterface.UpdateMyContactInfo(info);
        }

        public bool DeleteMyRecord(out string errorReason)
        {
            return _userInterface.DeleteMyRecord(out errorReason);
        }

        public bool ChangePassword(string currentPassword, string newPassword, out string errorReason)
        {
            return _userInterface.ChangePassword(currentPassword, newPassword, out errorReason);
        }

        public void Logout()
        {
            _userInterface.Logout();
        }

        public Building[] ListBuildings(string developerId)
        {
            return _userInterface.ListBuildings(developerId);
        }

        public Suite[] ListSuitesByBuiding(Building building)
        {
            return _userInterface.ListSuitesByBuiding(building);
        }
        #endregion

        public void Dispose()
        {
        }
    }
}