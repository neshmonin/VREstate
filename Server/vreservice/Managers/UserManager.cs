using System;
using NHibernate;
using Vre.Server.Dao;
using System.IO;
using System.Diagnostics;
using Vre.Server.RemoteService;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    internal class UserManager : GenericManager
    {
        public UserManager(ClientSession clientSession) : base(clientSession) { }

        public void Create(User.Role role, int developerId, LoginType type, string login, string password)
        {
            //RolePermissionCheck.CheckCreateUser(_session, ((developerId >= 0) ? (int?)developerId : null), role);
            RolePermissionCheck.CheckUserAccess(_session, ((developerId >= 0) ? (int?)developerId : null), role, 
                RolePermissionCheck.UserInfoAccessLevel.Administrative);

            createUserInt(role, developerId, type, login, password);

            ServiceInstances.Logger.Info("User ID={3} created new user: {0}[{1}] for developer ID={2}",
                                        type, login, developerId, _session.User.AutoID);
        }

        private void createUserInt(User.Role role, int developerId, LoginType type, string login, string password)
        {
            User newUser = null;

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session.DbSession))
            {
                string errorReason;
                
                if (User.IsEstateDeveloperTied(role))
                {
                    using (EstateDeveloperDao devDao = new EstateDeveloperDao(_session.DbSession))
                    {
                        if (devDao.GetById(developerId) != null) newUser = new User(developerId, role);
                        else throw new FileNotFoundException("The developer ID supplied does not exist.");
                    }
                }
                else
                {
                    newUser = new User(null, role);
                }

                if (newUser != null)
                {
                    using (UserDao dao = new UserDao(_session.DbSession))
                    {
                        dao.Create(newUser);

                        using (IAuthentication auth = new Authentication(_session.DbSession))
                        {
                            if (auth.CreateLogin(type, role, developerId, login, password, newUser.AutoID, out errorReason))
                                tran.Commit();
                            else
                                throw new InvalidOperationException(errorReason);
                        }
                    }
                }
            }
        }

        public static User Login(LoginType type, User.Role role, int estateDeveloperId, string login, string password)
        {
            User result = null;

            using (ISession session = NHibernateHelper.GetSession())
            {
                using (IAuthentication auth = new Authentication(session))
                {
                    int userId;
                    if (auth.AuthenticateUser(type, role, estateDeveloperId, login, password, out userId))
                    {
                        using (UserDao dao = new UserDao(session))
                        {
                            result = dao.GetById(userId);
                            if (result != null)  // update last login value
                            {
                                result.Touch();
                                dao.SafeUpdate(result);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public User Get(int userId)
        {
            User result = null;

            using (IAuthentication auth = new Authentication(_session.DbSession))
            {
                using (UserDao dao = new UserDao(_session.DbSession)) result = dao.GetById(userId, true);
            }

            if (null == result) throw new FileNotFoundException("User object does not exist.");
            //RolePermissionCheck.CheckGetUser(_session, result);
            RolePermissionCheck.CheckUserAccess(_session, result, RolePermissionCheck.UserInfoAccessLevel.Contact);
            return result;
        }

        public User Get(LoginType type, User.Role role, int estateDeveloperId, string login)
        {
            User result = null;

            using (IAuthentication auth = new Authentication(_session.DbSession))
            {
                int userId = auth.UserIdByLogin(type, role, estateDeveloperId, login);
                if (userId >= 0)
                {
                    using (UserDao dao = new UserDao(_session.DbSession)) result = dao.GetById(userId);
                }
            }

            if (null == result) throw new FileNotFoundException("User object does not exist.");
            //RolePermissionCheck.CheckGetUser(_session, result);
            RolePermissionCheck.CheckUserAccess(_session, result, RolePermissionCheck.UserInfoAccessLevel.Contact);
            return result;
        }

        //private static void validateOperation(User requestingUser, User result)
        //{
        //    Debug.Assert(requestingUser != null, "requestingUser is null!");
        //    Debug.Assert(result != null, "result is null!");

        //    // ROLE PERMISSION LOGIC
        //    if (result.Equals(requestingUser))
        //    {
        //        // all OK - same user
        //        return;
        //    }
        //    if (User.Role.SuperAdmin == result.UserRole)  // superadmin user record
        //    {
        //        if (requestingUser.UserRole == User.Role.SuperAdmin)  // only superadmins can read
        //        {
        //            // all OK - superadmin
        //            return;
        //        }
        //        else
        //        {
        //            throw new PermissionException(
        //                string.Format("User ID={0} with {1} role cannot access superadmin record.",
        //                requestingUser.AutoID, requestingUser.UserRole));
        //        }
        //    }
        //    else  // any non-superadmin use record
        //    {
        //        if (requestingUser.UserRole == User.Role.SuperAdmin)  // superadmins read everything
        //        {
        //            // all OK - superadmin
        //            return;
        //        }
        //        else if (requestingUser.UserRole == User.Role.DeveloperAdmin)  // develper admins read only their users
        //        {
        //            if (requestingUser.EstateDeveloperID == result.EstateDeveloperID)
        //            {
        //                // all OK - same estate developer
        //                return;
        //            }
        //            throw new PermissionException(
        //                string.Format("User ID={0} with DeveloperAdmin role at EDID={1} cannot access record from another Estate Developer.",
        //                requestingUser.AutoID, requestingUser.EstateDeveloperID));
        //        }
        //        else
        //        {
        //            throw new PermissionException(
        //                string.Format("User ID={0} with {1} role cannot access record.",
        //                requestingUser.AutoID, requestingUser.UserRole));
        //        }
        //    }
        //}

        public void ChangePassword(LoginType type, User.Role role, int estateDeveloperId, string login, 
            string currentPassword, string newPassword)
        {
			using (var tran = NHibernateHelper.OpenNonNestedTransaction(_session))
			{
				using (IAuthentication auth = new Authentication(_session.DbSession))
				{
					int userId;
					if (auth.AuthenticateUser(type, role, estateDeveloperId, login, currentPassword, out userId))
					{
						User u;
						using (UserDao dao = new UserDao(_session.DbSession)) u = dao.GetById(userId);

						//RolePermissionCheck.CheckDeleteUser(_session, u);
						RolePermissionCheck.CheckUserAccess(_session, u, RolePermissionCheck.UserInfoAccessLevel.Administrative);

						string errorReason;
						if (!auth.ChangePassword(userId, currentPassword, newPassword, out errorReason))
						{
							throw new PermissionException(errorReason);
						}

						u.PasswordChangeRequired = true;
						using (UserDao dao = new UserDao(_session.DbSession)) dao.SafeUpdate(u);
					}
					else
					{
						throw new PermissionException("Unknown login or bad current password");
					}
				}
				tran.Commit();
			}
        }

        public void ChangePassword(User user, string currentPassword, string newPassword)
        {
            //RolePermissionCheck.CheckDeleteUser(_session, user);
            RolePermissionCheck.CheckUserAccess(_session, user, RolePermissionCheck.UserInfoAccessLevel.Administrative);

			using (var tran = NHibernateHelper.OpenNonNestedTransaction(_session))
			{
				using (IAuthentication auth = new Authentication(_session.DbSession))
				{
					string errorReason;
					if (!auth.ChangePassword(user.AutoID, currentPassword, newPassword, out errorReason))
					{
						throw new PermissionException(errorReason);
					}
				}

				user.PasswordChangeRequired = false;
				using (UserDao dao = new UserDao(_session.DbSession)) dao.SafeUpdate(user);

				tran.Commit();
			}
        }

        public User[] List(User.Role role, int estateDeveloperId, string nameLookup, bool includeDeleted)
        {
            if (includeDeleted && (_session.User.UserRole != User.Role.SuperAdmin))
                includeDeleted = false;  // only superadmins can see deleted records!

            if (User.IsEstateDeveloperTied(role) && (estateDeveloperId < 0))
                throw new ArgumentException("Estate developer argument required");

            using (UserDao dao = new UserDao(_session.DbSession))
            {
                IList<User> result = dao.ListUsers(role, ((estateDeveloperId >= 0) ? (int?)estateDeveloperId : null),
                    nameLookup, includeDeleted);

                for (int idx = result.Count - 1; idx >= 0; idx--)
                {
                    RolePermissionCheck.UserInfoAccessLevel lvl = RolePermissionCheck.GetUserAccess(_session, result[idx]);

                    if (lvl < RolePermissionCheck.UserInfoAccessLevel.Contact)
                        result.RemoveAt(idx);
                }

                return NHibernateHelper.IListToArray<User>(result);
            }
        }

        public void Update(User user)
        {
            //RolePermissionCheck.CheckUpdateUser(_session, user);
            RolePermissionCheck.CheckUserAccess(_session, user, RolePermissionCheck.UserInfoAccessLevel.Administrative);

            using (UserDao dao = new UserDao(_session.DbSession))
            {
                if (!dao.SafeUpdate(user))
                    throw new StaleObjectStateException("Record was updated by other user", user.AutoID);

                ServiceInstances.Logger.Info("User ID={0} updated by ID={1} successfully.", user.AutoID, _session.User.AutoID);
            }
        }

        public void Delete(User user)
        {
            //RolePermissionCheck.CheckUpdateUser(_session, user);
            RolePermissionCheck.CheckUserAccess(_session, user, RolePermissionCheck.UserInfoAccessLevel.Administrative);

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session.DbSession))
            {
                using (UserDao dao = new UserDao(_session.DbSession))
                {
                    if (user.UserRole == User.Role.SuperAdmin)
                    {
                        if (_session.User.Equals(user)) throw new InvalidOperationException("Superadmin cannot commit suicide.");
                        if (dao.ListUsers(User.Role.SuperAdmin, null, null, false).Count < 2)
                        {
                            throw new InvalidOperationException("Cannot delete last active Superadmin.");
                        }
                    }
                    else if (user.UserRole == User.Role.DeveloperAdmin)
                    {
                        if (_session.User.Equals(user)) throw new InvalidOperationException("Developer Admin cannot commit suicide.");
                        if (dao.ListUsers(User.Role.DeveloperAdmin, user.EstateDeveloperID.Value, null, false).Count < 2)
                        {
                            throw new InvalidOperationException("Cannot delete last active Developer Admin.");
                        }
                    }

                    if (!dao.SafeDelete(user))
                        throw new StaleObjectStateException("Record was updated by other user", user.AutoID);
                }

                bool result;
                using (IAuthentication auth = new Authentication(_session.DbSession))
                {
                    result = auth.DropLogin(user.AutoID);
                }

                if (result)
                {
                    tran.Commit();
                    ServiceInstances.Logger.Info("User ID={0} dropped by ID={1} successfully.", user.AutoID, _session.User.AutoID);
                }
                else
                {
                    tran.Rollback();
                    throw new Exception("Consistency: user record ID=" + user.AutoID.ToString() + " references missing login.");
                }
            }
        }

        public void ConfirmPresetUsers()
        {
            using (UserDao dao = new UserDao(_session.DbSession))
            {
                if (!dao.HasActiveSuperAdmin())
                {
                    ServiceInstances.Logger.Warn("Database has no active superadmins; creating default superadmin.");

                    createUserInt(User.Role.SuperAdmin, -1, LoginType.Plain, "admin", "admin");
                    ServiceInstances.Logger.Info("Default Superadmin created.");
                }
            }

            //using (EstateDeveloperDao eddao = new EstateDeveloperDao(_session.DbSession))
            //{
            //    foreach (EstateDeveloper developer in eddao.GetAll())
            //    {
            //        using (UserDao dao = new UserDao(_session.DbSession))
            //        {
            //            if (0 == dao.List(User.Role.DeveloperAdmin, developer.AutoID, null, false).Length)
            //            {
            //            }
            //            if (0 == dao.List(User.Role.Buyer, developer.AutoID, null, false).Length)
            //            {
            //            }
            //        }
            //    }
            //}
        }

        //private static string generateDeveloperAdminLogin(UserDao dao, EstateDeveloper dev)
        //{
        //    string login;
        //    do
        //    {
        //    } 
        //}

        //public EstateDeveloper CreateEstateDeveloper(EstateDeveloper.Configuration configuration, string name)
        //{
        //    RolePermissionCheck.CheckCreateEstateDeveloper(_session, configuration, name);

        //    using (EstateDeveloperDao dao = new EstateDeveloperDao(_session.DbSession))
        //    {
        //        EstateDeveloper ed = new EstateDeveloper(configuration);
        //        ed.Name = name;
        //        dao.Create(ed);
        //        dao.Flush();
        //        ServiceInstances.Logger.Info("Estate developer type <{0}> created (ID={1}).", configuration, ed.AutoID);
        //        return ed;
        //    }
        //}

        //public EstateDeveloper[] ListDevelopers(bool includeDeleted)
        //{
        //    RolePermissionCheck.CheckListEstateDevelopers(_session);

        //    using (EstateDeveloperDao dao = new EstateDeveloperDao(_session.DbSession))
        //    {
        //        IList<EstateDeveloper> info = dao.GetAll();
        //        List<EstateDeveloper> result = new List<EstateDeveloper>(info.Count);

        //        foreach (EstateDeveloper ed in info) 
        //            if (!ed.Deleted || includeDeleted) result.Add(ed);

        //        return result.ToArray();
        //    }
        //}

        //public void DeleteEstateDeveloper(EstateDeveloper estateDeveloper)
        //{
        //    RolePermissionCheck.CheckDeleteEstateDeveloper(_session, estateDeveloper);

        //    using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session.DbSession))
        //    {
        //        int users = 0, buildings = 0;
        //        using (EstateDeveloperDao dao = new EstateDeveloperDao(_session.DbSession))
        //        {
        //            dao.Delete(estateDeveloper);
        //        }
        //        using (UserDao dao = new UserDao(_session.DbSession))
        //        {
        //            string errorReason;
        //            foreach (User u in dao.List(estateDeveloper.AutoID, null, false))
        //            {
        //                if (Delete(u, out errorReason)) users++;
        //                else throw new Exception("Failed to delete user: " + errorReason);
        //            }
        //            //users = dao.DeleteUsers(estateDeveloper.AutoID);
        //        }
        //        // no need to delete these: when estate developer is marked as deleted these became invisible for browsing
        //        //foreach (Site s in estateDeveloper.Sites)
        //        //    using (BuildingDao dao = new BuildingDao(_session))
        //        //{
        //        //    buildings = dao.DeleteBuildings(estateDeveloper.AutoID);
        //        //}
        //        tran.Commit();
        //        ServiceInstances.Logger.Info("Estate developer ID={0} deleted; {1} users and {2} buildings affected.", 
        //            estateDeveloper.AutoID, users, buildings);
        //    }
        //}

        public void GrantViewPermissionTo(string userId, bool revoke)
        {
            int granteeId;

            if (!int.TryParse(userId, out granteeId)) throw new ArgumentException("User ID is invalid");

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session.DbSession))
            {
                User grantee = Get(granteeId);
                if (null == grantee) throw new FileNotFoundException("User ID is not known.");
                
                GrantViewPermissionTo(grantee, revoke);

                tran.Commit();
            }
        }

        public void GrantViewPermissionTo(User user, bool revoke)
        {
            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session.DbSession))
            {
                using (UserDao dao = new UserDao(_session.DbSession))
                {
                    User requester = /*_session.User;*/ dao.GetById(_session.User.AutoID);
                    User accessGainer = /*user;*/ dao.GetById(user.AutoID);
                    bool changed = false;

                    if (requester.VisibleBy.Contains(accessGainer))
                    {
                        if (revoke)
                        {
                            requester.VisibleBy.Remove(accessGainer);
                            accessGainer.CanView.Remove(requester);
                            changed = true;
                        }
                    }
                    else
                    {
                        if (!revoke)
                        {
                            requester.VisibleBy.Add(accessGainer);
                            accessGainer.CanView.Add(requester);
                            changed = true;
                        }
                    }

                    if (changed)
                    {
                        if (!dao.SafeUpdate(accessGainer))
                            throw new StaleObjectStateException("Record was updated by other user", accessGainer.AutoID);
                        if (!dao.SafeUpdate(requester))
                            throw new StaleObjectStateException("Record was updated by other user", requester.AutoID);

                        tran.Commit();
                        ServiceInstances.Logger.Info("User {0} (ID={1}) {2} profile access to ID={3}.",
                            _session, _session.User.AutoID, revoke ? "revoked" : "granted", user.AutoID);
                    }
                }  // DAO
            }  // TRAN
        }

        public void LicenseUser(string userId, string siteId, DateTime endTimeUtc)
        {
            int licenseeId;
            if (!int.TryParse(userId, out licenseeId)) throw new ArgumentException("User ID is invalid");

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session.DbSession))
            {
                Site site;

                using (SiteDao sdao = new SiteDao(_session.DbSession))
                    site = sdao.GetById(siteId);
                if (null == site) throw new FileNotFoundException("Site ID is not known.");

                User licensee = Get(licenseeId);
                if (null == licensee) throw new FileNotFoundException("Licensee ID is not known.");

                RolePermissionCheck.CheckUserAccess(_session, licensee,
                    RolePermissionCheck.UserInfoAccessLevel.Administrative);

                UserLicense lic = licensee.EmitLicense(site, endTimeUtc, _session.User);

                _session.DbSession.Save(lic);
                Update(licensee);

                tran.Commit();

                ServiceInstances.Logger.Info("User {0}(ID={1}) licensed user ID={2} for site {3}(ID={4}) until (UTC){5}.",
                    _session, _session.User.AutoID,
                    licensee.AutoID,
                    site, site.AutoID,
                    endTimeUtc);
            }
        }

        public void AssignSellerToBuilding(string userId, string buildingId)
        {
            int licenseeId;
            if (!int.TryParse(userId, out licenseeId)) throw new ArgumentException("User ID is invalid");

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session.DbSession))
            {
                Building building;

                using (BuildingDao bdao = new BuildingDao(_session.DbSession))
                    building = bdao.GetById(buildingId);
                if (null == building) throw new FileNotFoundException("Building ID is not known.");

                User licensee = Get(licenseeId);
                if (null == licensee) throw new FileNotFoundException("Licensee ID is not known.");

                RolePermissionCheck.CheckUserAccess(_session, licensee,
                    RolePermissionCheck.UserInfoAccessLevel.Administrative);

                User prev = building.SellingBy;
                if (!licensee.Equals(prev))
                {
                    building.SellingBy = licensee;
                    licensee.ManagedBuildings.Add(building);

                    //Update(licensee);

                    //_session.DbSession.Save(building);

                    using (BuildingDao bdao = new BuildingDao(_session.DbSession))
                        if (!bdao.SafeUpdate(building))
                            throw new StaleObjectStateException("Record was updated by other user", building.AutoID);

                    tran.Commit();

                    ServiceInstances.Logger.Info("User {0}(ID={1}) assigned user ID={2} as seller for building {3}(ID={4}); previous seller was ID={5}.",
                        _session, _session.User.AutoID,
                        licensee.AutoID,
                        building, building.AutoID,
                        ((prev != null) ? prev.AutoID : -1));
                }
            }
        }

        public void AssignSellerToSuite(string userId, string suiteIdentity)
        {
            int licenseeId, suiteId;
            if (!int.TryParse(userId, out licenseeId)) throw new ArgumentException("User ID is invalid");
            if (!int.TryParse(suiteIdentity, out suiteId)) throw new ArgumentException("Suite ID is invalid");

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session.DbSession))
            {
                Suite suite;

                using (SuiteDao sdao = new SuiteDao(_session.DbSession))
                    suite = sdao.GetById(suiteId);
                if (null == suite) throw new FileNotFoundException("Suite ID is not known.");

                User licensee = Get(licenseeId);
                if (null == licensee) throw new FileNotFoundException("Licensee ID is not known.");

                RolePermissionCheck.CheckUserAccess(_session, licensee,
                    RolePermissionCheck.UserInfoAccessLevel.Administrative);

                User prev = suite.SellingBy;
                suite.SellingBy = licensee;

                using (SuiteDao sdao = new SuiteDao(_session.DbSession))
                    if (!sdao.SafeUpdate(suite))
                        throw new StaleObjectStateException("Record was updated by other user", suite.AutoID);

                tran.Commit();

                ServiceInstances.Logger.Info("User {0}(ID={1}) assigned user ID={2} as seller for suite {3}(ID={4}); previous seller was ID={5}.",
                    _session, _session.User.AutoID,
                    licensee.AutoID,
                    suite, suite.AutoID,
                    ((prev != null) ? prev.AutoID : -1));
            }
        }
    }
}