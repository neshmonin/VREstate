using System;
using System.Collections.Generic;
using System.IO;
using Vre.Server.RemoteService;
namespace Vre.Server.BusinessLogic
{
    internal static class RolePermissionCheck
    {
        //public static void Check(ClientSession session, object target, bool genericInfoOnly, bool writeAccess)
        //{
        //    if 
        //}

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

        public static void CheckUpdateBuilding(ClientSession session, Building building)
        {
            if (User.Role.SuperAdmin == session.User.UserRole)
            {
                // all OK
                ServiceInstances.Logger.Info("Superadmin ID={0} is updating building: ID={1}",
                    session.User.AutoID, building.AutoID);
                return;
            }
            else if (User.Role.DeveloperAdmin == session.User.UserRole)
            {
                if (session.User.EstateDeveloperID == building.ConstructionSite.Developer.AutoID)
                {
                    // all OK
                    ServiceInstances.Logger.Info("Estate developer ID={0} is updating building: ID={1}",
                        session.User.AutoID, building.AutoID);
                    return;
                }
                else
                {
                    throw new PermissionException("This operation is allowed to estate developer admins for same developer entity only.");
                }
            }
            else
            {
                throw new PermissionException("This operation is allowed to superadmins and estate developer admins only.");
            }
        }

		public static void CheckDeleteBuilding(ClientSession session, Building building)
		{
			if (User.Role.SuperAdmin == session.User.UserRole)
			{
				// all OK
				ServiceInstances.Logger.Info("Superadmin ID={0} is deleting building: ID={1}",
					session.User.AutoID, building.AutoID);
				return;
			}
			throw new PermissionException("This operation is allowed to superadmins only.");
		}
		#endregion

        #region suites
        public static void CheckUpdateSuite(ClientSession session, Suite suite)
        {
            if (User.Role.SuperAdmin == session.User.UserRole)
            {
                // all OK
                ServiceInstances.Logger.Info("Superadmin ID={0} is updating suite: ID={1}",
                    session.User.AutoID, suite.AutoID);
                return;
            }
            else if (User.Role.DeveloperAdmin == session.User.UserRole)
            {
                if (session.User.EstateDeveloperID == suite.Building.ConstructionSite.Developer.AutoID)
                {
                    // all OK
                    ServiceInstances.Logger.Info("Estate developer ID={0} is updating suite: ID={1}",
                        session.User.AutoID, suite.AutoID);
                    return;
                }
                else
                {
                    throw new PermissionException("This operation is allowed to estate developer admins for same developer entity only.");
                }
            }
            else if (User.Role.SellingAgent == session.User.UserRole)
            {
                // TODO: allow updating attached suites
                throw new PermissionException("This operation is allowed to superadmins and estate developer admins only.");

                //ServiceInstances.Logger.Info("Selling Agent ID={0} is updating suite: ID={1}",
                //    session.User.AutoID, suite.AutoID);
            }
            else
            {
                throw new PermissionException("This operation is allowed to superadmins and estate developer admins only.");
            }
        }
        #endregion

        #region users
        public enum UserInfoAccessLevel : int
        {
            /// <summary>
            /// No user information is available, even existence of the record cannot be confirmed.
            /// </summary>
            None = 0,
            /// <summary>
            /// Only user login is accessible.
            /// </summary>
            Minimal = 1,
            /// <summary>
            /// User login, name, photo and contact information is accessible.
            /// </summary>
            Contact = 2,
            /// <summary>
            /// Same as <see cref="Contact"/> plus information about objects viewed.
            /// </summary>
            Sales = 3,
            /// <summary>
            /// Same as <see cref="Sales"/> plus information about all user transactions.
            /// </summary>
            Transactional = 4,
            /// <summary>
            /// Full access to user information.
            /// </summary>
            Administrative = 5
        }

        private static Dictionary<User.Role, Dictionary<User.Role, Tuple<UserInfoAccessLevel, UserInfoAccessLevel, UserInfoAccessLevel, UserInfoAccessLevel>>> 
            _accessMatrix 
            = new Dictionary<User.Role, Dictionary<User.Role, Tuple<UserInfoAccessLevel, UserInfoAccessLevel, UserInfoAccessLevel, UserInfoAccessLevel>>>();

        private static void addUserAcl(User.Role caller, User.Role target,
            UserInfoAccessLevel defaultCase, UserInfoAccessLevel sameEd,
            UserInfoAccessLevel defaultGranted, UserInfoAccessLevel sameEdGranted)
        {
            Dictionary<User.Role, Tuple<UserInfoAccessLevel, UserInfoAccessLevel, UserInfoAccessLevel, UserInfoAccessLevel>>
                acl;

            if (!_accessMatrix.TryGetValue(caller, out acl))
            {
                acl = new Dictionary<User.Role, Tuple<UserInfoAccessLevel, UserInfoAccessLevel, UserInfoAccessLevel, UserInfoAccessLevel>>();
                _accessMatrix.Add(caller, acl);
            }

            // access values validation
            if (User.IsEstateDeveloperTied(caller) && User.IsEstateDeveloperTied(target))
            {
            }
            else
            {
                if (!defaultCase.Equals(sameEd)) 
                    throw new ArgumentException("Estate Developer related rules defined for non-developer role (D).");
                if (!defaultGranted.Equals(sameEdGranted)) 
                    throw new ArgumentException("Estate Developer related rules defined for non-developer role (G).");
            }
            if (defaultGranted < defaultCase)
                throw new ArgumentException("Granted access level cannot be less than non-granted (D).");
            if (sameEdGranted < sameEd)
                throw new ArgumentException("Granted access level cannot be less than non-granted (S).");


            Tuple<UserInfoAccessLevel, UserInfoAccessLevel, UserInfoAccessLevel, UserInfoAccessLevel>
                ac = new Tuple<UserInfoAccessLevel,UserInfoAccessLevel,UserInfoAccessLevel,UserInfoAccessLevel>(
                    defaultCase, sameEd, defaultGranted, sameEdGranted);

			if (acl.ContainsKey(target)) throw new ArgumentException("Access level rule redefinition for " + caller + "->" + target);
            acl.Add(target, ac);
        }

        private static UserInfoAccessLevel getUserAL(User.Role caller, User.Role target,
            bool sameEd, bool isAccessGranted)
        {
            try
            {
                Tuple<UserInfoAccessLevel, UserInfoAccessLevel, UserInfoAccessLevel, UserInfoAccessLevel>
                    ac = _accessMatrix[caller][target];

                if (sameEd)
                {
                    if (isAccessGranted) return ac.Item4;
                    else return ac.Item2;
                }
                else
                {
                    if (isAccessGranted) return ac.Item3;
                    else return ac.Item1;
                }
            }
            catch (KeyNotFoundException)
            {
                return UserInfoAccessLevel.Minimal;
            }
        }

        /// <summary>
        /// THIS METHOD IS NOT THREAD-SAFE
        /// </summary>
        public static void FillAccessMatrix()
        {
            if (_accessMatrix.Count > 0) return;

            addUserAcl(User.Role.SuperAdmin, User.Role.Buyer,
                UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative,
                UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative);
            addUserAcl(User.Role.SuperAdmin, User.Role.DeveloperAdmin,
                UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative,
                UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative);
            addUserAcl(User.Role.SuperAdmin, User.Role.Kiosk,
                UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative,
                UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative);
            addUserAcl(User.Role.SuperAdmin, User.Role.SalesPerson,
                UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative,
                UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative);
            addUserAcl(User.Role.SuperAdmin, User.Role.SellingAgent,
                UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative,
                UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative);
			addUserAcl(User.Role.SuperAdmin, User.Role.BuyingAgent,
				UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative,
				UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative);
			addUserAcl(User.Role.SuperAdmin, User.Role.Subcontractor,
                UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative,
                UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative);
            addUserAcl(User.Role.SuperAdmin, User.Role.SuperAdmin,
                UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative,
                UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative);
            addUserAcl(User.Role.SuperAdmin, User.Role.Visitor,
                UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative,
                UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative);
			addUserAcl(User.Role.SuperAdmin, User.Role.Anonymous,
				UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative,
				UserInfoAccessLevel.Administrative, UserInfoAccessLevel.Administrative);

            addUserAcl(User.Role.DeveloperAdmin, User.Role.Buyer,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.DeveloperAdmin, User.Role.DeveloperAdmin,
                UserInfoAccessLevel.None, UserInfoAccessLevel.Contact,
                UserInfoAccessLevel.None, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.DeveloperAdmin, User.Role.Kiosk,
                UserInfoAccessLevel.None, UserInfoAccessLevel.Contact,
                UserInfoAccessLevel.None, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.DeveloperAdmin, User.Role.SalesPerson,
                UserInfoAccessLevel.None, UserInfoAccessLevel.Administrative,
                UserInfoAccessLevel.None, UserInfoAccessLevel.Administrative);
            addUserAcl(User.Role.DeveloperAdmin, User.Role.SellingAgent,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,  // Should be more here?
                UserInfoAccessLevel.Transactional, UserInfoAccessLevel.Transactional);
			addUserAcl(User.Role.DeveloperAdmin, User.Role.BuyingAgent,
				UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,  // Should be more here?
				UserInfoAccessLevel.Transactional, UserInfoAccessLevel.Transactional);
			addUserAcl(User.Role.DeveloperAdmin, User.Role.Subcontractor,
                UserInfoAccessLevel.None, UserInfoAccessLevel.Administrative,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Administrative);
            addUserAcl(User.Role.DeveloperAdmin, User.Role.SuperAdmin,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.DeveloperAdmin, User.Role.Visitor,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.DeveloperAdmin, User.Role.Anonymous,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None);

            addUserAcl(User.Role.Kiosk, User.Role.Buyer,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None);
            addUserAcl(User.Role.Kiosk, User.Role.DeveloperAdmin,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None);
            addUserAcl(User.Role.Kiosk, User.Role.Kiosk,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None);
            addUserAcl(User.Role.Kiosk, User.Role.SalesPerson,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None);
            addUserAcl(User.Role.Kiosk, User.Role.SellingAgent,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None);
			addUserAcl(User.Role.Kiosk, User.Role.BuyingAgent,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None);
			addUserAcl(User.Role.Kiosk, User.Role.Subcontractor,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None);
            addUserAcl(User.Role.Kiosk, User.Role.SuperAdmin,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None);
            addUserAcl(User.Role.Kiosk, User.Role.Visitor,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None);
			addUserAcl(User.Role.Kiosk, User.Role.Anonymous,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None);

            addUserAcl(User.Role.SalesPerson, User.Role.Buyer,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
                UserInfoAccessLevel.Sales, UserInfoAccessLevel.Sales);
            addUserAcl(User.Role.SalesPerson, User.Role.DeveloperAdmin,
                UserInfoAccessLevel.None, UserInfoAccessLevel.Contact,
                UserInfoAccessLevel.None, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.SalesPerson, User.Role.Kiosk,
                UserInfoAccessLevel.None, UserInfoAccessLevel.Contact,
                UserInfoAccessLevel.None, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.SalesPerson, User.Role.SalesPerson,
                UserInfoAccessLevel.None, UserInfoAccessLevel.Contact,
                UserInfoAccessLevel.None, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.SalesPerson, User.Role.SellingAgent,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.SalesPerson, User.Role.BuyingAgent,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.SalesPerson, User.Role.Subcontractor,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Contact,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.SalesPerson, User.Role.SuperAdmin,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.SalesPerson, User.Role.Visitor,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
                UserInfoAccessLevel.Sales, UserInfoAccessLevel.Sales);
			addUserAcl(User.Role.SalesPerson, User.Role.Anonymous,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None);

            addUserAcl(User.Role.SellingAgent, User.Role.Buyer,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
                UserInfoAccessLevel.Sales, UserInfoAccessLevel.Sales);
            addUserAcl(User.Role.SellingAgent, User.Role.DeveloperAdmin,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.SellingAgent, User.Role.Kiosk,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.SellingAgent, User.Role.SalesPerson,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.SellingAgent, User.Role.SellingAgent,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.SellingAgent, User.Role.BuyingAgent,
				UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
				UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.SellingAgent, User.Role.Subcontractor,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.SellingAgent, User.Role.SuperAdmin,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.SellingAgent, User.Role.Visitor,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
                UserInfoAccessLevel.Sales, UserInfoAccessLevel.Sales);
			addUserAcl(User.Role.SellingAgent, User.Role.Anonymous,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None);

			addUserAcl(User.Role.BuyingAgent, User.Role.Buyer,
				UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
				UserInfoAccessLevel.Sales, UserInfoAccessLevel.Sales);
			addUserAcl(User.Role.BuyingAgent, User.Role.DeveloperAdmin,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.BuyingAgent, User.Role.Kiosk,
				UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
				UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.BuyingAgent, User.Role.SalesPerson,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.BuyingAgent, User.Role.SellingAgent,
				UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
				UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.BuyingAgent, User.Role.BuyingAgent,
				UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
				UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.BuyingAgent, User.Role.Subcontractor,
				UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
				UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.BuyingAgent, User.Role.SuperAdmin,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.BuyingAgent, User.Role.Visitor,
				UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
				UserInfoAccessLevel.Sales, UserInfoAccessLevel.Sales);
			addUserAcl(User.Role.BuyingAgent, User.Role.Anonymous,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None);

			addUserAcl(User.Role.Subcontractor, User.Role.Buyer,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None);
            addUserAcl(User.Role.Subcontractor, User.Role.DeveloperAdmin,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Contact,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.Subcontractor, User.Role.Kiosk,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None);
            addUserAcl(User.Role.Subcontractor, User.Role.SalesPerson,
                UserInfoAccessLevel.None, UserInfoAccessLevel.Contact,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.Subcontractor, User.Role.SellingAgent,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.Subcontractor, User.Role.BuyingAgent,
				UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
				UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.Subcontractor, User.Role.Subcontractor,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Contact,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.Subcontractor, User.Role.SuperAdmin,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.Subcontractor, User.Role.Visitor,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.Subcontractor, User.Role.Anonymous,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None);

            addUserAcl(User.Role.Buyer, User.Role.Buyer,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.Buyer, User.Role.DeveloperAdmin,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.Buyer, User.Role.Kiosk,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.Buyer, User.Role.SalesPerson,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.Buyer, User.Role.SellingAgent,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.Buyer, User.Role.BuyingAgent,
				UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact,
				UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.Buyer, User.Role.Subcontractor,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.Buyer, User.Role.SuperAdmin,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.Buyer, User.Role.Visitor,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.Buyer, User.Role.Anonymous,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None);

            addUserAcl(User.Role.Visitor, User.Role.Buyer,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.Visitor, User.Role.DeveloperAdmin,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.Visitor, User.Role.Kiosk,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.Visitor, User.Role.SalesPerson,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.Visitor, User.Role.SellingAgent,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.Visitor, User.Role.BuyingAgent,
				UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact,
				UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.Visitor, User.Role.Subcontractor,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.Visitor, User.Role.SuperAdmin,
                UserInfoAccessLevel.None, UserInfoAccessLevel.None,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
            addUserAcl(User.Role.Visitor, User.Role.Visitor,
                UserInfoAccessLevel.Minimal, UserInfoAccessLevel.Minimal,
                UserInfoAccessLevel.Contact, UserInfoAccessLevel.Contact);
			addUserAcl(User.Role.Visitor, User.Role.Anonymous,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None);

			addUserAcl(User.Role.Anonymous, User.Role.Buyer,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None);
			addUserAcl(User.Role.Anonymous, User.Role.DeveloperAdmin,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None);
			addUserAcl(User.Role.Anonymous, User.Role.Kiosk,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None);
			addUserAcl(User.Role.Anonymous, User.Role.SalesPerson,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None);
			addUserAcl(User.Role.Anonymous, User.Role.SellingAgent,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None);
			addUserAcl(User.Role.Anonymous, User.Role.BuyingAgent,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None);
			addUserAcl(User.Role.Anonymous, User.Role.Subcontractor,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None);
			addUserAcl(User.Role.Anonymous, User.Role.SuperAdmin,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None);
			addUserAcl(User.Role.Anonymous, User.Role.Anonymous,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None);
			addUserAcl(User.Role.Anonymous, User.Role.Visitor,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None,
				UserInfoAccessLevel.None, UserInfoAccessLevel.None);

			// cross-reference check
			foreach (var t in Enum.GetValues(typeof(User.Role)))
			{
				Dictionary<User.Role, Tuple<UserInfoAccessLevel, UserInfoAccessLevel, UserInfoAccessLevel, UserInfoAccessLevel>> acl;
				
				if (!_accessMatrix.TryGetValue((User.Role)t, out acl))
					throw new ApplicationException("User Access Marix is missing ACL for " + t);

				foreach (var tt in Enum.GetValues(typeof(User.Role)))
					if (!acl.ContainsKey((User.Role)tt))
						throw new ApplicationException("User Access Marix ACL for " + t + " is missing rule for " + tt);
			}
        }

        private static void genericUserControlCheck(ClientSession session, User user, UserInfoAccessLevel level)
        {
            if (user.Equals(session.User))
            {
                // all OK - same user
                return;
            }

            bool sameEd = false;
            if (session.User.EstateDeveloperID.HasValue)
                sameEd = session.User.EstateDeveloperID.Equals(user.EstateDeveloperID);

            UserInfoAccessLevel lvl = getUserAL(session.User.UserRole, user.UserRole,
                sameEd, user.VisibleBy.Contains(session.User));

            if (lvl >= level) return;

            if (UserInfoAccessLevel.None == lvl) throw new FileNotFoundException("User object does not exist.");
            else throw new PermissionException("This operation is not allowed.");
        }

        private static void genericUserControlCheck(ClientSession session, 
            int? targetUserEstateDeveloperId, User.Role targetUserRole, UserInfoAccessLevel level)
        {
            bool sameEd = false;
            if (session.User.EstateDeveloperID.HasValue)
                sameEd = session.User.EstateDeveloperID.Equals(targetUserEstateDeveloperId);

            UserInfoAccessLevel lvl = getUserAL(session.User.UserRole, targetUserRole,
                sameEd, false);

            if (lvl >= level) return;

            if (UserInfoAccessLevel.None == lvl) throw new FileNotFoundException("User object does not exist.");
            else throw new PermissionException("This operation is not allowed.");
        }

        //private static void genericUserControlCheck(ClientSession session,
        //    int? targetEstateDeveloperId, User.Role targetRole, bool readAccess, bool fullAccess)
        //{
        //    switch (session.User.UserRole)
        //    {
        //        case User.Role.SuperAdmin:
        //            // superadmins have full control
        //            return;

        //        case User.Role.DeveloperAdmin:
        //            switch (targetRole)
        //            {
        //                case User.Role.SalesPerson:
        //                case User.Role.Subcontractor:
        //                    if (session.User.EstateDeveloperID.Equals(targetEstateDeveloperId))
        //                        // developer admins can fully control their sales personnell
        //                        return;
        //                    else
        //                        break;
        //            }
        //            break;

        //        default:
        //            if (readAccess)
        //            {
        //                if (fullAccess)
        //                {
        //                    if (
        //                }
        //            }

        //        case User.Role.SalesPerson:
        //        case User.Role.SellingAgent:
        //            switch (targetRole)
        //            {
        //                case User.Role.Buyer:
                            
        //            }
        //    }



        //    switch (targetRole)
        //    {
        //        case User.Role.SuperAdmin:
        //            if (targetEstateDeveloperId.HasValue)
        //                throw new ArgumentException("Superadmins cannot have estate developer ID.");

        //            if (session.User.UserRole == User.Role.SuperAdmin)  // only superadmins can read
        //            {
        //                // all OK - superadmin
        //                break;
        //            }
        //            else
        //            {
        //                throw new PermissionException("This operation is allowed to superadmins only.");
        //                //throw new PermissionException(
        //                //    string.Format("User ID={0} with {1} role cannot access superadmin record.",
        //                //    session.User.AutoID, session.User.UserRole));
        //            }

        //        case User.Role.SellingAgent:
        //        case User.Role.SalesPerson:
        //            if (readAccess)
        //            {
        //                // sales personnell could be viewed by anyone
        //                break;
        //            }
        //            else
        //            {
        //                if ((session.User.UserRole == User.Role.DeveloperAdmin)
        //                    && session.User.EstateDeveloperID.Equals(targetEstateDeveloperId))
        //                    // developer admins have full control of their sales personnell
        //                    break;
        //                if ((session.User.UserRole == User.Role.SuperAdmin)
        //                    // 
        //                    break;
        //            }

        //    }

        //    if (User.Role.SuperAdmin == targetRole)  // superadmin user record
        //    {
        //        if (targetEstateDeveloperId.HasValue)
        //            throw new ArgumentException("Superadmins cannot have estate developer ID.");

        //        if (session.User.UserRole == User.Role.SuperAdmin)  // only superadmins can read
        //        {
        //            // all OK - superadmin
        //            return;
        //        }
        //        else
        //        {
        //            throw new PermissionException("This operation is allowed to superadmins only.");
        //            //throw new PermissionException(
        //            //    string.Format("User ID={0} with {1} role cannot access superadmin record.",
        //            //    session.User.AutoID, session.User.UserRole));
        //        }
        //    }
        //    else  // any non-superadmin use record
        //    {
        //        if (session.User.UserRole == User.Role.SuperAdmin)  // superadmins read everything
        //        {
        //            // all OK - superadmin
        //            return;
        //        }
        //        else if (session.User.UserRole == User.Role.DeveloperAdmin)  // develper admins read only their users
        //        {
        //            if (session.User.EstateDeveloperID.Equals(targetEstateDeveloperId) && readAccess)
        //            {
        //                // all OK - same estate developer
        //                return;
        //            }
        //            throw new PermissionException("This operation is allowed within same estate developer only."); 
        //            //throw new PermissionException(
        //            //    string.Format("User ID={0} with DeveloperAdmin role at EDID={1} cannot access record from another Estate Developer.",
        //            //    session.User.AutoID, session.User.EstateDeveloperID));
        //        }
        //        else if (session.User.UserRole == User.Role.SalesPerson)
        //        {
        //            if (session.User.EstateDeveloperID.Equals(targetEstateDeveloperId)
        //                && (targetRole == User.Role.Buyer))
        //            {
        //                return;
        //            }
        //            else
        //            {
        //                throw new PermissionException("This operation is allowed against buyers of same estate developer only.");
        //            }
        //        }
        //        else if (((targetRole == User.Role.SalesPerson) || (targetRole == User.Role.SellingAgent))
        //            && readAccess)
        //        {
        //            // sales personnell could be viewed by anyone
        //            return;
        //        }
        //        else
        //        {
        //            throw new PermissionException("This operation is allowed to estate developer admins and sales only.");
        //            //throw new PermissionException(
        //            //    string.Format("User ID={0} with {1} role cannot access record.",
        //            //    session.User.AutoID, session.User.UserRole));
        //        }
        //    }
        //}

        public static UserInfoAccessLevel GetUserAccess(ClientSession session, User user)
        {
            bool sameEd = false;
            if (session.User.EstateDeveloperID.HasValue)
                sameEd = session.User.EstateDeveloperID.Equals(user.EstateDeveloperID);

            return getUserAL(session.User.UserRole, user.UserRole,
                sameEd, user.VisibleBy.Contains(session.User));
        }

        public static void CheckUserAccess(ClientSession session, User user, UserInfoAccessLevel level)
        {
            genericUserControlCheck(session, user, level);
        }

        public static void CheckUserAccess(ClientSession session, int? targetUserEstateDeveloperId, User.Role targetUserRole, 
            UserInfoAccessLevel level)
        {
            genericUserControlCheck(session, targetUserEstateDeveloperId, targetUserRole, level);
        }

        //public static void CheckListUsers(ClientSession session)
        //{
        //    if (session.User.UserRole != User.Role.SuperAdmin)
        //    {
        //        throw new PermissionException("This operation is allowed to superadmins only.");
        //    }
        //}

        //public static void CheckListUsers(ClientSession session, int? estateDeveloperIdFilter)
        //{
        //    if (User.Role.SuperAdmin == session.User.UserRole)
        //    {
        //        return;
        //    }
        //    else if (User.Role.DeveloperAdmin == session.User.UserRole)
        //    {
        //        if (session.User.EstateDeveloperID.Equals(estateDeveloperIdFilter))
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            throw new PermissionException("This operation is allowed to estate developer admins only.");
        //        }
        //    }
        //    else
        //    {
        //        throw new PermissionException("This operation is allowed to superadmins only.");
        //    }
        //}

        //public static void CheckListUsers(ClientSession session, int? estateDeveloperIdFilter, User.Role roleFilter)
        //{
        //    genericUserControlCheck(session, estateDeveloperIdFilter, roleFilter, true);
        //}

        //public static void CheckCreateUser(ClientSession session, int? estateDeveloperIdFilter, User.Role roleFilter)
        //{
        //    genericUserControlCheck(session, estateDeveloperIdFilter, roleFilter, false);
        //}

        //public static void CheckUpdateUser(ClientSession session, User user)
        //{
        //    genericUserControlCheck(session, user, false);
        //}

        //public static void CheckDeleteUser(ClientSession session, User user)
        //{
        //    genericUserControlCheck(session, user, false);
        //}

        public static void CheckUserChangeLogin(ClientSession session)
        {
            if (session.User.UserRole == User.Role.SellingAgent) return;

            throw new PermissionException("This operation is not allowed.");
        }
        #endregion

        #region view orders
		/// <summary>
		/// 
		/// </summary>
		/// <param name="session">Web session requesting access</param>
		/// <param name="targetUser">User who shall be the owner of the ViewOrder</param>
		/// <param name="building">Bulding associated with the property referenced by ViewOrder</param>
		/// <returns>true if immediate create allowed; false if payment is required</returns>
        public static bool CheckCreateViewOrder(ClientSession session, User targetUser, Building building)
        {
            // superadmin can make a listing for anyone
            if (session.User.UserRole == User.Role.SuperAdmin) return true;

            // developer admin can make a listing for anyone for a building/suite belonging to his estate developer
            if (session.User.UserRole == User.Role.DeveloperAdmin)
            {
                if (null == building) return false;
                if (building.ConstructionSite.Developer.AutoID.Equals(session.User.EstateDeveloperID)) return false;
            }

            //// selling agent can make a listing for himself
            //if ((session.User.UserRole == User.Role.SellingAgent)
            //    && (targetUser.Equals(session.User))) return;

			// web sale
			if ((session.User.UserRole == User.Role.Visitor)
				&& (targetUser.UserRole == User.Role.Anonymous))
				return false;

            throw new PermissionException("This operation is not allowed.");
        }

        public static bool CheckUpdateViewOrder(ClientSession session, User targetUser)
        {
            return CheckCreateViewOrder(session, targetUser, null);  // same permissions
        }

        public static void CheckDeleteViewOrder(ClientSession session, User targetUser)
        {
            CheckCreateViewOrder(session, targetUser, null);  // same permissions
        }
        #endregion

		#region named search filters
		public static void CheckCreateNamedSearchFilter(ClientSession session)
		{
			if (session.User.UserRole == User.Role.SuperAdmin) return;
			if (session.User.UserRole == User.Role.BuyingAgent) return;

			throw new PermissionException("This operation is not allowed.");
		}

		public static void CheckUpdateNamedSearchFilter(ClientSession session, NamedSearchFilter target)
		{
			if (session.User.UserRole == User.Role.SuperAdmin) return;

			if (session.User.AutoID == target.OwnerId) return;
		}

		public static void CheckDeleteNamedSearchFilter(ClientSession session, NamedSearchFilter target)
		{
			if (session.User.UserRole == User.Role.SuperAdmin) return;

			if (session.User.AutoID == target.OwnerId) return;
		}

		public static void CheckReadNamedSearchFilter(ClientSession session, NamedSearchFilter target)
		{
			if (session.User.UserRole == User.Role.SuperAdmin) return;

			if (session.User.AutoID == target.OwnerId) return;
		}
		#endregion
	}
}