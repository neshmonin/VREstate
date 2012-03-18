using Vre.Server.RemoteService;
using System;
namespace Vre.Server.BusinessLogic
{
    internal static class RolePermissionCheck
    {
        public static void CheckReadDeletedObjects(ClientSession session)
        {
            if (session.User.UserRole != User.Role.SuperAdmin)
                throw new PermissionException("This operation is allowed to superadmins only.");
        }

        #region estate developers
        private static void genericEstateDeveloperCheck(ClientSession session)
        {
            if (session.User.UserRole != User.Role.SuperAdmin)
                throw new PermissionException("This operation is allowed to superadmins only.");
        }

        public static void CheckListEstateDevelopers(ClientSession session)
        {
            genericEstateDeveloperCheck(session);
        }

        public static void CheckCreateEstateDeveloper(ClientSession session,
            EstateDeveloper.Configuration cfg, string edName)
        {
            genericEstateDeveloperCheck(session);

            ServiceInstances.Logger.Info("Superadmin ID={0} is creating estate developer: '{1}'; type <{2}>",
                session.User.AutoID, edName, cfg);
        }

        public static void CheckUpdateEstateDeveloper(ClientSession session, EstateDeveloper ed)
        {
            genericEstateDeveloperCheck(session);

            ServiceInstances.Logger.Info("Superadmin ID={0} is updating estate developer: ID={1}",
                session.User.AutoID, ed.AutoID);
        }

        public static void CheckDeleteEstateDeveloper(ClientSession session, EstateDeveloper ed)
        {
            genericEstateDeveloperCheck(session);

            ServiceInstances.Logger.Info("Superadmin ID={0} is deleting estate developer: ID={1}",
                session.User.AutoID, ed.AutoID);
        }
        #endregion

        #region sites
        public static void CheckListSites(ClientSession session)
        {
            // everyone allowed
        }
        #endregion

        #region buildings
        public static void CheckListBuildings(ClientSession session)
        {
            // everyone allowed
        }
        #endregion

        #region users
        private static void genericUserControlCheck(ClientSession session, User user)
        {
            if (user.Equals(session.User))
            {
                // all OK - same user
                return;
            }
            genericUserControlCheck(session, user.EstateDeveloperID, user.UserRole);
        }

        private static void genericUserControlCheck(ClientSession session, 
            int? targetEstateDeveloperId, User.Role targetRole)
        {
            if (User.Role.SuperAdmin == targetRole)  // superadmin user record
            {
                if (targetEstateDeveloperId.HasValue)
                    throw new ArgumentException("Superadmins cannot have estate developer ID.");

                if (session.User.UserRole == User.Role.SuperAdmin)  // only superadmins can read
                {
                    // all OK - superadmin
                    return;
                }
                else
                {
                    throw new PermissionException("This operation is allowed to superadmins only.");
                    //throw new PermissionException(
                    //    string.Format("User ID={0} with {1} role cannot access superadmin record.",
                    //    session.User.AutoID, session.User.UserRole));
                }
            }
            else  // any non-superadmin use record
            {
                if (session.User.UserRole == User.Role.SuperAdmin)  // superadmins read everything
                {
                    // all OK - superadmin
                    return;
                }
                else if (session.User.UserRole == User.Role.DeveloperAdmin)  // develper admins read only their users
                {
                    if (session.User.EstateDeveloperID.Equals(targetEstateDeveloperId))
                    {
                        // all OK - same estate developer
                        return;
                    }
                    throw new PermissionException("This operation is allowed within same estate developer only."); 
                    //throw new PermissionException(
                    //    string.Format("User ID={0} with DeveloperAdmin role at EDID={1} cannot access record from another Estate Developer.",
                    //    session.User.AutoID, session.User.EstateDeveloperID));
                }
                else if (session.User.UserRole == User.Role.SalesPerson)
                {
                    if (session.User.EstateDeveloperID.Equals(targetEstateDeveloperId)
                        && (targetRole == User.Role.Buyer))
                    {
                        return;
                    }
                    else
                    {
                        throw new PermissionException("This operation is allowed against buyers of same estate developer only.");
                    }
                }
                else
                {
                    throw new PermissionException("This operation is allowed to estate developer admins and sales only.");
                    //throw new PermissionException(
                    //    string.Format("User ID={0} with {1} role cannot access record.",
                    //    session.User.AutoID, session.User.UserRole));
                }
            }
        }

        public static void CheckGetUser(ClientSession session, User user)
        {
            genericUserControlCheck(session, user);
        }

        public static void CheckListUsers(ClientSession session)
        {
            if (session.User.UserRole != User.Role.SuperAdmin)
            {
                throw new PermissionException("This operation is allowed to superadmins only.");
            }
        }

        public static void CheckListUsers(ClientSession session, int? estateDeveloperIdFilter)
        {
            if (User.Role.SuperAdmin == session.User.UserRole)
            {
                return;
            }
            else if (User.Role.DeveloperAdmin == session.User.UserRole)
            {
                if (session.User.EstateDeveloperID.Equals(estateDeveloperIdFilter))
                {
                    return;
                }
                else
                {
                    throw new PermissionException("This operation is allowed to estate developer admins only.");
                }
            }
            else
            {
                throw new PermissionException("This operation is allowed to superadmins only.");
            }
        }

        public static void CheckListUsers(ClientSession session, int? estateDeveloperIdFilter, User.Role roleFilter)
        {
            genericUserControlCheck(session, estateDeveloperIdFilter, roleFilter);
        }

        public static void CheckCreateUser(ClientSession session, int? estateDeveloperIdFilter, User.Role roleFilter)
        {
            genericUserControlCheck(session, estateDeveloperIdFilter, roleFilter);
        }

        public static void CheckUpdateUser(ClientSession session, User user)
        {
            genericUserControlCheck(session, user);
        }

        public static void CheckDeleteUser(ClientSession session, User user)
        {
            genericUserControlCheck(session, user);
        }
        #endregion
    }
}