using System;
using NHibernate;
using Vre.Server.Dao;
using System.IO;
using System.Diagnostics;
using Vre.Server.RemoteService;

namespace Vre.Server.BusinessLogic
{
    public class UserManager : GenericManager
    {
        public UserManager(ClientSession clientSession) : base(clientSession) { }

        public bool CreateUser(User.Role role, int developerId, LoginType type, string login, string password, out string errorReason)
        {
            // ROLE PERMISSION LOGIC
            if ((User.Role.DeveloperAdmin == role) || (User.Role.SuperAdmin == role))
            {
                if (_session.User.UserRole != User.Role.SuperAdmin)
                {
                    errorReason = "Only Superadmin can create administrators.";
                    return false;
                }
            }
            else //if ((User.Role.SalesPerson == role) || (User.Role.Subcontractor == role))
            {
                if (_session.User.UserRole == User.Role.SuperAdmin)
                {
                    // all OK
                }
                else if (_session.User.UserRole == User.Role.DeveloperAdmin)
                {
                    if (_session.User.EstateDeveloperID != developerId)
                    {
                        errorReason = "Can only create users for same Estate Developer as creator is.";
                        return false;
                    }
                }
                else
                {
                    errorReason = "Only administrators can create users.";
                    return false;
                }
            }
            if ((role == User.Role.SuperAdmin) && (developerId >= 0))
            {
                errorReason = "Cannot create Superadmin for Estate Developer.";
                return false;
            }

            if (createUserInt(role, developerId, type, login, password, out errorReason))
            {
                ServiceInstances.Logger.Info("User ID={3} created new user: {0}[{1}] for developer ID={2}",
                                            type, login, developerId, _session.User.AutoID);
                return true;
            }
            return false;
        }

        private bool createUserInt(User.Role role, int developerId, LoginType type, string login, string password, out string errorReason)
        {
            User newUser = null;

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session.DbSession))
            {
                bool loginCreated = false;

                // ROLE PERMISSION LOGIC
                if (role != User.Role.SuperAdmin)
                {
                    errorReason = null;  // no need for this; but compiler wants it :)
                    using (EstateDeveloperDao devDao = new EstateDeveloperDao(_session.DbSession))
                    {
                        if (devDao.GetById(developerId) != null) newUser = new User(developerId, role);
                        else errorReason = "The developer ID supplied does not exist.";
                    }
                }
                else
                {
                    errorReason = null;  // no need for this; but compiler wants it :)
                    newUser = new User(null, role);
                }

                if (newUser != null)
                {
                    using (UserDao dao = new UserDao(_session.DbSession))
                    {
                        dao.Create(newUser);
                        try
                        {
                            using (IAuthentication auth = new Authentication(_session.DbSession))
                            {
                                loginCreated = auth.CreateLogin(type, login, password, newUser.AutoID, out errorReason);
                            }
                        }
                        catch (Exception ex)
                        {
                            ServiceInstances.Logger.Error("Login create failed (0): {0}.", ex);
                            newUser = null;
                            errorReason = "Operation failed (0).  Try again later.";
                        }
                        if (!loginCreated) newUser = null;
                    }
                }

                if (loginCreated) tran.Commit();
                else tran.Rollback();
            }

            return (newUser != null);
        }

        public static User Login(LoginType type, string login, string password)
        {
            User result = null;

            using (ISession session = NHibernateHelper.GetSession())
            {
                using (IAuthentication auth = new Authentication(session))
                {
                    int userId;
                    if (auth.AuthenticateUser(type, login, password, out userId))
                    {
                        using (UserDao dao = new UserDao(session)) result = dao.GetById(userId);
                    }
                }
            }

            return result;
        }

        public User GetUser(int userId)
        {
            User result = null;

            using (IAuthentication auth = new Authentication(_session.DbSession))
            {
                using (UserDao dao = new UserDao(_session.DbSession)) result = dao.GetById(userId, true);
            }

            if (null == result) throw new FileNotFoundException("User object does not exist.");
            validateOperation(_session.User, result);
            return result;
        }

        public User GetUser(LoginType type, string login)
        {
            User result = null;

            using (IAuthentication auth = new Authentication(_session.DbSession))
            {
                int userId = auth.UserIdByLogin(type, login);
                if (userId >= 0)
                {
                    using (UserDao dao = new UserDao(_session.DbSession)) result = dao.GetById(userId);
                }
            }

            if (null == result) throw new FileNotFoundException("User object does not exist.");
            validateOperation(_session.User, result);
            return result;
        }

        private static void validateOperation(User requestingUser, User result)
        {
            Debug.Assert(requestingUser != null, "requestingUser is null!");
            Debug.Assert(result != null, "result is null!");

            // ROLE PERMISSION LOGIC
            if (result.Equals(requestingUser))
            {
                // all OK - same user
                return;
            }
            if (User.Role.SuperAdmin == result.UserRole)  // superadmin user record
            {
                if (requestingUser.UserRole == User.Role.SuperAdmin)  // only superadmins can read
                {
                    // all OK - superadmin
                    return;
                }
                else
                {
                    throw new PermissionException(
                        string.Format("User ID={0} with {1} role cannot access superadmin record.",
                        requestingUser.AutoID, requestingUser.UserRole));
                }
            }
            else  // any non-superadmin use record
            {
                if (requestingUser.UserRole == User.Role.SuperAdmin)  // superadmins read everything
                {
                    // all OK - superadmin
                    return;
                }
                else if (requestingUser.UserRole == User.Role.DeveloperAdmin)  // develper admins read only their users
                {
                    if (requestingUser.EstateDeveloperID == result.EstateDeveloperID)
                    {
                        // all OK - same estate developer
                        return;
                    }
                    throw new PermissionException(
                        string.Format("User ID={0} with DeveloperAdmin role at EDID={1} cannot access record from another Estate Developer.",
                        requestingUser.AutoID, requestingUser.EstateDeveloperID));
                }
                else
                {
                    throw new PermissionException(
                        string.Format("User ID={0} with {1} role cannot access record.",
                        requestingUser.AutoID, requestingUser.UserRole));
                }
            }
        }

        public bool ChangePassword(LoginType type, string login, string currentPassword, string newPassword, out string errorReason)
        {
            using (IAuthentication auth = new Authentication(_session.DbSession))
            {
                return auth.ChangePassword(type, login, currentPassword, newPassword, out errorReason);
            }
        }

        public User[] ListUsers(User.Role role, int estateDeveloperId, string nameLookup, bool includeDeleted)
        {
            using (UserDao dao = new UserDao(_session.DbSession))
            {
                return dao.ListUsers(role, estateDeveloperId, nameLookup, includeDeleted);
            }
        }

        public bool UpdateUser(User user, out string errorReason)
        {
            validateOperation(_session.User, user);

            using (UserDao dao = new UserDao(_session.DbSession))
            {
                if (!dao.SafeUpdate(user))
                {
                    errorReason = "Record was updated by other user";
                    return false;
                }
                else
                {
                    errorReason = null;
                    ServiceInstances.Logger.Info("User ID={0} updates by ID={1} successfully.", user.AutoID, _session.User.AutoID);
                }
            }
            return true;
        }

        public bool DeleteUser(User user, out string errorReason)
        {
            bool result = false;

            validateOperation(_session.User, user);
                
            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session.DbSession))
            {
                using (UserDao dao = new UserDao(_session.DbSession))
                {
                    // ROLE PERMISSION LOGIC
                    if (user.UserRole == User.Role.SuperAdmin)
                    {
                        if (dao.ListSuperAdmins(null, false).Length > 1)
                        {
                            errorReason = null;
                            result = true;
                        }
                        else
                        {
                            errorReason = "Cannot delete last active Superadmin.";
                        }
                    }
                    // ROLE PERMISSION LOGIC
                    else if (user.UserRole == User.Role.DeveloperAdmin)
                    {
                        if (dao.ListUsers(User.Role.DeveloperAdmin, user.EstateDeveloperID.Value, null, false).Length > 1)
                        {
                            errorReason = null;
                            result = true;
                        }
                        else
                        {
                            errorReason = "Cannot delete last active Developer Admin.";
                        }
                    }
                    else
                    {
                        errorReason = null;
                        result = true;
                    }

                    if (result)
                    {
                        if (!dao.SafeDelete(user))
                        {
                            errorReason = "Record was updated by other user";
                            result = false;
                        }
                    }
                }
                if (result)
                {
                    using (IAuthentication auth = new Authentication(_session.DbSession))
                    {
                        result = auth.DropLogin(user.AutoID);
                    }
                }

                if (result)
                {
                    tran.Commit();
                    ServiceInstances.Logger.Info("User ID={0} dropped by ID={1} successfully.", user.AutoID, _session.User.AutoID);
                }
                else
                {
                    tran.Rollback();
                }
            }

            return result;
        }

        public void ConfirmPresetUsers()
        {
            using (UserDao dao = new UserDao(_session.DbSession))
            {
                if (!dao.HasActiveSuperAdmin())
                {
                    ServiceInstances.Logger.Warn("Database has no active superadmins; creating default superadmin.");

                    string errorText;
                    if (createUserInt(User.Role.SuperAdmin, -1, LoginType.Plain, "admin", "admin", out errorText))
                    {
                        ServiceInstances.Logger.Info("Default Superadmin created.");
                    }
                    else
                    {
                        ServiceInstances.Logger.Fatal("Unable to create Superadmin: {0}.", errorText);
                    }
                }
            }
        }

        public EstateDeveloper CreateEstateDeveloper(EstateDeveloper.Configuration configuration)
        {
            // TODO: Permission checks and logging.

            using (EstateDeveloperDao dao = new EstateDeveloperDao(_session.DbSession))
            {
                EstateDeveloper ed = new EstateDeveloper(configuration);
                dao.Create(ed);
                dao.Flush();
                ServiceInstances.Logger.Info("Estate developer type <{0}> created (ID={1}).", configuration, ed.AutoID);
                return ed;
            }
        }

        public void DeleteEstateDeveloper(EstateDeveloper estateDeveloper)
        {
            // TODO: Permission checks and logging.

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session.DbSession))
            {
                int users = 0, buildings = 0;
                using (EstateDeveloperDao dao = new EstateDeveloperDao(_session.DbSession))
                {
                    dao.Delete(estateDeveloper);
                }
                using (UserDao dao = new UserDao(_session.DbSession))
                {
                    string errorReason;
                    foreach (User u in dao.ListUsers(estateDeveloper.AutoID, null, false))
                    {
                        if (DeleteUser(u, out errorReason)) users++;
                        else throw new Exception("Failed to delete user: " + errorReason);
                    }
                    //users = dao.DeleteUsers(estateDeveloper.AutoID);
                }
                // no need to delete these: when estate developer is marked as deleted these became invisible for browsing
                //foreach (Site s in estateDeveloper.Sites)
                //    using (BuildingDao dao = new BuildingDao(_session))
                //{
                //    buildings = dao.DeleteBuildings(estateDeveloper.AutoID);
                //}
                tran.Commit();
                ServiceInstances.Logger.Info("Estate developer ID={0} deleted; {1} users and {2} buildings affected.", 
                    estateDeveloper.AutoID, users, buildings);
            }
        }
    }
}