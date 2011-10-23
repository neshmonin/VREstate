using System;
using System.Collections.Generic;
using System.ServiceModel;
using Vre.Server.BusinessLogic;

namespace Vre.Client
{
    /// <summary>
    /// Factory, generating proxies to server-side functionality.
    /// </summary>
    public class Factory
    {
        public static IUser CreateUser()
        {
            return new UserWrapper();
        }
        
        public static IBuyer CreateBuyer()
        {
            return new BuyerWrapper();
        }
        public static ISalesperson CreateSalesperson()
        {
            return new SalespersonWrapper();
        }
        public static ISubcontractor CreateSubcontractor()
        {
            return new SubcontractorWrapper();
        }
        public static IDeveloperAdmin CreateDeveloperAdmin()
        {
            return new DeveloperAdminWrapper();
        }
    }

    internal class UserWrapper : ClientBase<IUser>, IUser
    {
        public UserWrapper() : base("VreServiceVisitor") { }
        public void Dispose() { Close(); }

        public bool CreateBuyer(string developerId, LoginType type, string login, string password, out string errorReason)
        {
            return base.Channel.CreateBuyer(developerId, type, login, password, out errorReason);
        }

        //public ContactInfo GetMyContactInfo()
        //{
        //    return base.Channel.GetMyContactInfo();
        //}

        //public DataUpdateResult UpdateMyContactInfo(ContactInfo info)
        //{
        //    return base.Channel.UpdateMyContactInfo(info);
        //}

        //public bool DeleteMyRecord(out string errorReason)
        //{
        //    return base.Channel.DeleteMyRecord(out errorReason);
        //}

        //public bool Login(LoginType type, string userId, string password, out string errorReason)
        //{
        //    return base.Channel.Login(type, userId, password, out errorReason);
        //}

        //public bool ChangePassword(string currentPassword, string newPassword, out string errorReason)
        //{
        //    return base.Channel.ChangePassword(currentPassword, newPassword, out errorReason);
        //}

        //public void Logout()
        //{
        //    base.Channel.Logout();
        //}
        
        public Building[] ListBuildings(string developerId)
        {
            return base.Channel.ListBuildings(developerId);
        }

        public Suite[] ListSuitesByBuiding(Building building)
        {
            return base.Channel.ListSuitesByBuiding(building);
        }
    }

    internal class BuyerWrapper : ClientBase<IBuyer>, IBuyer
    {
        public BuyerWrapper() : base("VreServiceBuyer") { }
        public void Dispose() { Close(); }

        public bool CreateBuyer(string developerId, LoginType type, string login, string password, out string errorReason)
        {
            return base.Channel.CreateBuyer(developerId, type, login, password, out errorReason);
        }

        public ContactInfo GetMyContactInfo()
        {
            return base.Channel.GetMyContactInfo();
        }

        public DataUpdateResult UpdateMyContactInfo(ContactInfo info)
        {
            return base.Channel.UpdateMyContactInfo(info);
        }

        public bool DeleteMyRecord(out string errorReason)
        {
            return base.Channel.DeleteMyRecord(out errorReason);
        }

        public bool Login(LoginType type, string userId, string password, out string errorReason)
        {
            return base.Channel.Login(type, userId, password, out errorReason);
        }

        public bool ChangePassword(string currentPassword, string newPassword, out string errorReason)
        {
            return base.Channel.ChangePassword(currentPassword, newPassword, out errorReason);
        }

        public void Logout()
        {
            base.Channel.Logout();
        }

        public List<Account> GetAccounts()
        {
            return base.Channel.GetAccounts();
        }

        public ContactInfo GetContactInfo()
        {
            return base.Channel.GetContactInfo();
        }

        public Building[] ListBuildings(string developerId)
        {
            return base.Channel.ListBuildings(developerId);
        }

        public Suite[] ListSuitesByBuiding(Building building)
        {
            return base.Channel.ListSuitesByBuiding(building);
        }
    }

    internal class SalespersonWrapper : ClientBase<ISalesperson>, ISalesperson
    {
        public SalespersonWrapper() : base("VreServiceSalesperson") { }
        public void Dispose() { Close(); }

        public bool CreateBuyer(string developerId, LoginType type, string login, string password, out string errorReason)
        {
            return base.Channel.CreateBuyer(developerId, type, login, password, out errorReason);
        }

        public ContactInfo GetMyContactInfo()
        {
            return base.Channel.GetMyContactInfo();
        }

        public DataUpdateResult UpdateMyContactInfo(ContactInfo info)
        {
            return base.Channel.UpdateMyContactInfo(info);
        }

        public bool DeleteMyRecord(out string errorReason)
        {
            return base.Channel.DeleteMyRecord(out errorReason);
        }

        public bool Login(LoginType type, string userId, string password, out string errorReason)
        {
            return base.Channel.Login(type, userId, password, out errorReason);
        }

        public bool ChangePassword(string currentPassword, string newPassword, out string errorReason)
        {
            return base.Channel.ChangePassword(currentPassword, newPassword, out errorReason);
        }

        public void Logout()
        {
            base.Channel.Logout();
        }

        public Building[] ListBuildings(string developerId)
        {
            return base.Channel.ListBuildings(developerId);
        }

        public Suite[] ListSuitesByBuiding(Building building)
        {
            return base.Channel.ListSuitesByBuiding(building);
        }
    }

    internal class SubcontractorWrapper : ClientBase<ISubcontractor>, ISubcontractor
    {
        public SubcontractorWrapper() : base("VreServiceSubcontractor") { }
        public void Dispose() { Close(); }

        public bool CreateBuyer(string developerId, LoginType type, string login, string password, out string errorReason)
        {
            return base.Channel.CreateBuyer(developerId, type, login, password, out errorReason);
        }

        public ContactInfo GetMyContactInfo()
        {
            return base.Channel.GetMyContactInfo();
        }

        public DataUpdateResult UpdateMyContactInfo(ContactInfo info)
        {
            return base.Channel.UpdateMyContactInfo(info);
        }

        public bool DeleteMyRecord(out string errorReason)
        {
            return base.Channel.DeleteMyRecord(out errorReason);
        }

        public bool Login(LoginType type, string userId, string password, out string errorReason)
        {
            return base.Channel.Login(type, userId, password, out errorReason);
        }

        public bool ChangePassword(string currentPassword, string newPassword, out string errorReason)
        {
            return base.Channel.ChangePassword(currentPassword, newPassword, out errorReason);
        }

        public void Logout()
        {
            base.Channel.Logout();
        }

        public IList<Option> ListOptionsByBuilding(int buildingId)
        {
            return base.Channel.ListOptionsByBuilding(buildingId);
        }

        public IList<Option> ListOptionsByBuildingAndSuite(int buildingId, int suiteId)
        {
            return base.Channel.ListOptionsByBuildingAndSuite(buildingId, suiteId);
        }

        public void UpdateCutOffDate(int optionId, int buildingId, DateTime cutOffDate)
        {
            base.Channel.UpdateCutOffDate(optionId, buildingId, cutOffDate);
        }

        public Building[] ListBuildings(string developerId)
        {
            return base.Channel.ListBuildings(developerId);
        }

        public Suite[] ListSuitesByBuiding(Building building)
        {
            return base.Channel.ListSuitesByBuiding(building);
        }
    }

    internal class DeveloperAdminWrapper : ClientBase<IDeveloperAdmin>, IDeveloperAdmin
    {
        public DeveloperAdminWrapper() : base("VreServiceDeveloperAdmin") { }
        public void Dispose() { Close(); }

        public bool CreateBuyer(string developerId, LoginType type, string login, string password, out string errorReason)
        {
            return base.Channel.CreateBuyer(developerId, type, login, password, out errorReason);
        }

        public ContactInfo GetMyContactInfo()
        {
            return base.Channel.GetMyContactInfo();
        }

        public DataUpdateResult UpdateMyContactInfo(ContactInfo info)
        {
            return base.Channel.UpdateMyContactInfo(info);
        }

        public bool DeleteMyRecord(out string errorReason)
        {
            return base.Channel.DeleteMyRecord(out errorReason);
        }

        public bool Login(LoginType type, string userId, string password, out string errorReason)
        {
            return base.Channel.Login(type, userId, password, out errorReason);
        }

        public bool ChangePassword(string currentPassword, string newPassword, out string errorReason)
        {
            return base.Channel.ChangePassword(currentPassword, newPassword, out errorReason);
        }

        public void Logout()
        {
            base.Channel.Logout();
        }

        public User[] ListUsers(User.Role role)
        {
            return base.Channel.ListUsers(role);
        }

        public IList<Option> ListOptions(bool enabledOnly)
        {
            return base.Channel.ListOptions(enabledOnly);
        }

        public void CreateOption(Option option)
        {
            base.Channel.CreateOption(option);
        }

        public void UpdateOption(Option option)
        {
            base.Channel.UpdateOption(option);
        }

        public User GetBuyerDetails(User buyer)
        {
            return base.Channel.GetBuyerDetails(buyer);
        }

        public void CancelTransaction(Transaction tran)
        {
            base.Channel.CancelTransaction(tran);
        }

        public IList<Option> ListOptionsByBuilding(int buildingId)
        {
            return base.Channel.ListOptionsByBuilding(buildingId);
        }

        public IList<Option> ListOptionsByBuildingAndSuite(int buildingId, int suiteId)
        {
            return base.Channel.ListOptionsByBuildingAndSuite(buildingId, suiteId);
        }

        public void UpdateCutOffDate(int optionId, int buildingId, DateTime cutOffDate)
        {
            base.Channel.UpdateCutOffDate(optionId, buildingId, cutOffDate);
        }

        public Building[] ListBuildings(string developerId)
        {
            return base.Channel.ListBuildings(developerId);
        }

        public Suite[] ListSuitesByBuiding(Building building)
        {
            return base.Channel.ListSuitesByBuiding(building);
        }

        public bool CreateSubcontractor(LoginType type, string login, string password, out string errorReason)
        {
            return base.Channel.CreateSubcontractor(type, login, password, out errorReason);
        }

        public bool CreateSalesperson(LoginType type, string login, string password, out string errorReason)
        {
            return base.Channel.CreateSalesperson(type, login, password, out errorReason);
        }

        public bool CreateAdmin(LoginType type, string login, string password, out string errorReason)
        {
            return base.Channel.CreateAdmin(type, login, password, out errorReason);
        }

        public bool DeleteUser(User user, out string errorReason)
        {
            return base.Channel.DeleteUser(user, out errorReason);
        }
    }
}
