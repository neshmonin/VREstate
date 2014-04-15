using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;
using Vre.Server.HttpService;
using Vre.Server.Messaging;
using System.Text;
using Vre.Server.Task;
using System.Drawing;
using Vre.Server.Util;

namespace Vre.Server.RemoteService
{
    internal class ProgramService
    {
        public const string ServicePathPrefix = "program";

        public static void ProcessClientRequest(IServiceRequest request)
        {
            string command = request.Request.Query["q"];
            
            if (!string.IsNullOrWhiteSpace(command))
            {
                if (command.Equals("login") && (request.Request.Type == RequestType.Get))
                {
                    login(request);
                    return;
                }
                else if (command.Equals("sessionrenew") && (request.Request.Type == RequestType.Get))
                {
                    sessionRenew(request);
                    return;
                }
                else if (command.Equals("chpwd") && (request.Request.Type == RequestType.Get))
                {
                    changePassword(request);
                    return;
                }
				else if (command.Equals("recover") && (request.Request.Type == RequestType.Get))
				{
					recoverPassword(request);
					return;
				}
				else if (command.Equals("chlogin") && (request.Request.Type == RequestType.Get))
                {
                    changeLogin(request);
                    return;
                }
                else if (command.Equals("grantaccess") && (request.Request.Type == RequestType.Get))
                {
                    grantAccess(request);
                    return;
                }
                else if (command.Equals("license") && (request.Request.Type == RequestType.Get))
                {
                    licenseUser(request);
                    return;
                }
                else if (command.Equals("assignseller") && (request.Request.Type == RequestType.Get))
                {
                    assignSeller(request);
                    return;
                }
                else if (command.Equals("register") && (request.Request.Type == RequestType.Get))
                {
                    register(request);
                    return;
                }
                else if (command.Equals("check") && (request.Request.Type == RequestType.Get))
                {
                    check(request);
                    return;
                }
                else if (command.Equals("salesmessage", StringComparison.InvariantCultureIgnoreCase) 
                    && ((request.Request.Type == RequestType.Insert) || (request.Request.Type == RequestType.Update)))
                {
                    sendSalesMessage(request);
                    return;
                }
				else if (command.Equals("getmlsinfo", StringComparison.InvariantCultureIgnoreCase)
					&& (request.Request.Type == RequestType.Get))
				{
					getMlsInfo(request);
					return;
				}
				else if ((command.Equals("store_temp_file") || command.Equals("storetempfile")) && (request.Request.Type == RequestType.Insert))
				{
					storeTempFile(request);
					return;
				}
				else if ((command.Equals("set_user_photo") || command.Equals("setuserphoto")) && (request.Request.Type == RequestType.Insert))
				{
					setUserPhoto(request);
					return;
				}
                else if (command.Equals("credit")
                    && ((request.Request.Type == RequestType.Insert) || (request.Request.Type == RequestType.Update) || (request.Request.Type == RequestType.Get)))
                {
                    credit(request);
                    return;
                }
            }

            throw new ArgumentException("Program command not understood.");
        }

		private static void ensureSessionExists(IServiceRequest request)
		{
			if (null == request.UserInfo.Session) throw new InvalidOperationException("A session required for this operation");
		}

        private static void sendSalesMessage(IServiceRequest request)
        {
            string subject = request.Request.Query.GetParam("subject", "Automatic sales notification");
            //string subject = request.Request.Data.GetProperty("subject", "Automatic sales notification");
            string body = request.Request.RawData != null ? Encoding.UTF8.GetString(request.Request.RawData) 
                : (request.Request.Data != null ? JavaScriptHelper.ClientDataToJson(request.Request.Data) : "Error: message text was not provided.");
            bool testMode = "true".Equals(request.Request.Query.GetParam("test", "false"));
			if (!testMode) testMode = "true".Equals(request.Request.Query.GetParam("testMode", "false"));  // OBSOLETE URI
            string recipient = request.Request.Query.GetParam("receiver", testMode ? "eugene.simonov@3dcondox.com" : "sales@3dcondox.com");

            //body += "\r\n\r\nNote: this message was automatically generated by 3D Condo Explorer Server in a response to a sales request.";

            ServiceInstances.EmailSender.Send(new Vre.Server.Messaging.Message(
				testMode ? Sender.Server : Sender.ECommerce, recipient, subject, body));

            ServiceInstances.Logger.Info("Sent sales msg {0}->{1} via {2}",
                Enum.GetName(typeof(Sender), testMode ? Sender.Server : Sender.ECommerce), recipient, request.Request.EndPoint);

            request.Response.ResponseCode = HttpStatusCode.OK;
        }

        private static void login(IServiceRequest request)
        {
            // parse required arguments
            //
            LoginType loginType = parseLoginType(request.Request.Query);
            User.Role role;
            string estateDeveloperId = request.Request.Query["ed"];
            string login = request.Request.Query["uid"];
            string password = request.Request.Query["pwd"];
            string sessionId = null;

            // role with default of superadmin
            if (!Enum.TryParse<User.Role>(request.Request.Query["role"], true, out role)) role = User.Role.SuperAdmin;

            var brokerageId = request.Request.Query.GetParam("bid", -1);

            // estate developer ID with default of none and reference by name support
            int edId = DataService.ResolveDeveloperId(null, estateDeveloperId);

            // authenticate
            //
            if ((!string.IsNullOrWhiteSpace(login)) && (!string.IsNullOrWhiteSpace(password)))
            {
                sessionId = ServiceInstances.SessionStore.LoginUser(request.Request.EndPoint, 
                    loginType, role, (edId >= 0) ? edId : brokerageId, login, password);
            }

            // test user validity
            //
            //if (role == User.Role.SellingAgent)
            //{
            //    ClientSession cs = ServiceInstances.SessionStore[sessionId];
            //    if (!cs.User.HasAnyLicense())
            //    {
            //        ServiceInstances.Logger.Info("User {0} has no valid licenses; rejecting login.", cs);
            //        cs = null;
            //        ServiceInstances.SessionStore.DropSession(sessionId);
            //        sessionId = null;
            //    }
            //}

            // produce output
            //
            if (sessionId != null)
            {
				if (!sessionId.StartsWith("---"))
				{
					var user = ServiceInstances.SessionStore[sessionId].User;
					request.Response.ResponseCode = HttpStatusCode.OK;
					request.Response.Data = new ClientData();
					request.Response.Data.Add("sid", sessionId);
					request.Response.Data.Add("keepalivePeriodSec", ServiceInstances.SessionStore.ClientKeepalivePeriodSec);
					request.Response.Data.Add("userId", user.AutoID);
					request.Response.Data.Add("passwordChangeRequired", user.PasswordChangeRequired);
				}
				else
				{
					request.Response.ResponseCode = HttpStatusCode.ServiceUnavailable;
					request.Response.Data = new ClientData();
					request.Response.Data.Add("waitSec", sessionId.Substring(3));
				}
            }
            else
            {
                request.Response.ResponseCode = HttpStatusCode.Forbidden;
                request.Response.ResponseCodeDescription = "Invalid logn type, login name or password.";
            }
        }

        private static void changePassword(IServiceRequest request)
        {
			ensureSessionExists(request);

            // parse required arguments
            //
            LoginType loginType = parseLoginType(request.Request.Query);
            string login = request.Request.Query["uid"];
            User.Role role;
            int estateDeveloperId = DataService.ResolveDeveloperId(null, request.Request.Query["ed"]);
            int brokerageId = request.Request.Query.GetParam("bid", -1);
            string password = request.Request.Query["pwd"];
            string newPassword = request.Request.Query["npwd"];
            if (!Enum.TryParse<User.Role>(request.Request.Query["role"], true, out role)) role = User.Role.Visitor;

            using (ISession session = NHibernateHelper.GetSession())
            {
                using (UserManager manager = new UserManager(request.UserInfo.Session))
                {
                    if (string.IsNullOrEmpty(login))  // self-service :)
                    {
                        manager.ChangePassword(request.UserInfo.Session.User, password, newPassword);
                    }
                    else
                    {
                        manager.ChangePassword(loginType, role, estateDeveloperId >= 0 ? estateDeveloperId : brokerageId, 
                            login, password, newPassword);
                    }
                }
            }

            request.Response.ResponseCode = HttpStatusCode.OK;
        }

        private static void changeLogin(IServiceRequest request)
        {
			ensureSessionExists(request);

			string newLogin = request.Request.Query["newlogin"];
			if (string.IsNullOrEmpty(newLogin)) newLogin = request.Request.Query["newLogin"];  // OBSOLETE URI
            RolePermissionCheck.CheckUserChangeLogin(request.UserInfo.Session);
            ReverseRequestService.InitiateLoginChange(request.Request, request.UserInfo.Session, newLogin);
            request.Response.ResponseCode = HttpStatusCode.OK;
        }

        private static void sessionRenew(IServiceRequest request)
        {
            // produce output
            //
            if (request.UserInfo.Session != null)
            {
                request.Response.ResponseCode = HttpStatusCode.OK;
            }
            else
            {
                request.Response.ResponseCode = HttpStatusCode.BadRequest;
                request.Response.ResponseCodeDescription = "Session ID not provided.";
            }
        }

        private static void grantAccess(IServiceRequest request)
        {
			ensureSessionExists(request);

			string granteeId = request.Request.Query["user"];
            bool grant = request.Request.Query.GetParam("grant", "true").Equals("true");

            using (UserManager um = new UserManager(request.UserInfo.Session))
            {
                um.GrantViewPermissionTo(granteeId, !grant);
            }

            request.Response.ResponseCode = HttpStatusCode.OK;
        }

        private static void licenseUser(IServiceRequest request)
        {
			ensureSessionExists(request);

			string licenseeId = request.Request.Query["user"];
            string siteId = request.Request.Query["site"];
            DateTime limit;

            limit = HttpServiceBase.ParseDateTimeParam(request.Request.Query["endtime"], null);

            using (UserManager um = new UserManager(request.UserInfo.Session))
            {
                um.LicenseUser(licenseeId, siteId, limit.ToUniversalTime());
            }

            request.Response.ResponseCode = HttpStatusCode.OK;
        }

        private static void assignSeller(IServiceRequest request)
        {
			ensureSessionExists(request);

			string userId = request.Request.Query["user"];
            string targetId;
            bool processed = false;

            if (!processed)
            {
                targetId = request.Request.Query["suite"];
                if (!string.IsNullOrWhiteSpace(targetId))
                {
                    using (UserManager um = new UserManager(request.UserInfo.Session))
                    {
                        um.AssignSellerToSuite(userId, targetId);
                    }
                    processed = true;
                }
            }

            if (!processed)
            {
                targetId = request.Request.Query["building"];
                if (!string.IsNullOrWhiteSpace(targetId))
                {
                    using (UserManager um = new UserManager(request.UserInfo.Session))
                    {
                        um.AssignSellerToBuilding(userId, targetId);
                    }
                    processed = true;
                }
            }

            if (processed)
                request.Response.ResponseCode = HttpStatusCode.OK;
            else
                throw new ArgumentException("Object to assign to is not defined or is unknown");
        }

        private static void register(IServiceRequest request)
        {
            string entity = request.Request.Query["entity"];

            if (string.IsNullOrWhiteSpace(entity)) throw new ArgumentException("entity not defined");

            if (entity.Equals("vieworder", StringComparison.InvariantCultureIgnoreCase))
            {
                registerViewOrder(request);
            }
            else if (entity.Equals("user"))
            {
                registerUser(request);
            }
            else if (entity.Equals("brokerage"))
            {
            }
            else
            {
                throw new ArgumentException("unknown entity");
            }
        }

        private static void check(IServiceRequest request)
        {
            string entity = request.Request.Query["entity"];

            if (string.IsNullOrWhiteSpace(entity)) throw new ArgumentException("entity not defined");

            if (entity.Equals("address"))
            {
                checkAddress(request);
            }
            else
            {
                throw new ArgumentException("unknown entity");
            }
        }

        private static void registerViewOrder(IServiceRequest request)
        {
            DateTime expiresOn;
            ViewOrder.ViewOrderProduct product;
            ViewOrder.ViewOrderOptions options;
            string paymentRefId = HttpUtility.UrlDecode(request.Request.Query["pr"]);
            string productUrl = HttpUtility.UrlDecode(request.Request.Query["evt_url"]);
            string mlsId = request.Request.Query["mls_id"];
            string mlsUrl = HttpUtility.UrlDecode(request.Request.Query["mls_url"]);
			string note = HttpUtility.UrlDecode(request.Request.Query["note"]);
            string ownerId = request.Request.Query["ownerid"];
			if (string.IsNullOrEmpty(ownerId)) ownerId = request.Request.Query["ownerId"];  // OBSOLETE URI
			string anonymousOwnerEmail = request.Request.Query["owneremail"];
			if (string.IsNullOrEmpty(anonymousOwnerEmail)) anonymousOwnerEmail = request.Request.Query["ownerEmail"];  // OBSOLETE URI
			string strNetAmountToPay = request.Request.Query["paymentpending"];
			if (string.IsNullOrEmpty(strNetAmountToPay)) strNetAmountToPay = request.Request.Query["paymentPending"];  // OBSOLETE URI
			Money netAmountToPay = Money.Zero;
            ViewOrder.SubjectType targetType;
            int targetId;

			if (request.UserInfo.Session != null)
			{
				switch (request.UserInfo.Session.User.UserRole)
				{
					case User.Role.Agent:
						registerAgentViewOrder(request, mlsId, paymentRefId);
						return;

					case User.Role.BuyingAgent:
						registerBuyingAgentViewOrder(request, mlsId, paymentRefId);
						return;
				}
			}

			expiresOn = getViewOrderTimeArgument(request);

            if (!request.Request.HasProductType()) throw new ArgumentException("Product type missing");
			product = request.Request.GetProductType();
			options = request.Request.GetProductOptions();

            //if (string.IsNullOrWhiteSpace(paymentRefId)) throw new ArgumentException("Required parameter missing");

            if (options == ViewOrder.ViewOrderOptions.ExternalTour)
            {
                if (string.IsNullOrWhiteSpace(productUrl))
                    throw new ArgumentException("External Virtual Tour reference not provided");
            }

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(request.UserInfo.Session.DbSession))
            {
                User targetUser = null;

                // use override for view order owner: anonymous user
				if (!string.IsNullOrWhiteSpace(anonymousOwnerEmail))
				{
					using (var dao = new UserDao(request.UserInfo.Session.DbSession))
					{
						var list = dao.GetByEmailAndRole(User.Role.Anonymous, anonymousOwnerEmail);
						if (1 == list.Count)
						{
							targetUser = list[0];
						}
						else if (0 == list.Count)
						{
							targetUser = new User(null, User.Role.Anonymous);
							targetUser.PrimaryEmailAddress = anonymousOwnerEmail;
							dao.Create(targetUser);
						}
						else
						{
							throw new ApplicationException("DBC-05384");
						}
					}

					if (!Money.TryParse(strNetAmountToPay, out netAmountToPay))
						throw new ArgumentException("'paymentPending' missing or invalid");
				}

				// use override for view order owner: explicit user ID
				if (null == targetUser)
				{
					int userId = -1;
					if (!string.IsNullOrWhiteSpace(ownerId))
					{
						if (!int.TryParse(ownerId, out userId)) userId = -1;
					}

					using (UserDao dao = new UserDao(request.UserInfo.Session.DbSession))
						targetUser = dao.GetById(userId);
				}

                if (null == targetUser) targetUser = request.UserInfo.Session.User;  // if unknown/not specified - make a view order for caller


                // ========================================================
                // BE CAREFUL WITH PERMISSION CHECKS IN THIS "IF" STATEMENT
                //
				bool immediateCreate;
                if (!request.Request.HasPropertyId() || !request.Request.HasPropertyType())
                {
                    // view by address lookup
                    //
                    UpdateableBase to = AddressHelper.ParseGeographicalAddressToModel(request.Request.Query, request.UserInfo.Session.DbSession);

                    Suite s = to as Suite;
                    if (s != null)
                    {
                        if (s.Status != Suite.SalesStatus.Sold) throw new ObjectExistsException("Suite status in not SOLD");
                        targetType = ViewOrder.SubjectType.Suite;
                        targetId = s.AutoID;
						immediateCreate = RolePermissionCheck.CheckCreateViewOrder(request.UserInfo.Session, targetUser, s.Building);
                    }
                    else
                    {
                        Building b = to as Building;
                        if (b != null)
                        {
                            if (product != ViewOrder.ViewOrderProduct.Building3DLayout)
                            {
                                if (b.Status != Building.BuildingStatus.Sold) throw new ObjectExistsException("Building status in not SOLD");
                            }
                            targetType = ViewOrder.SubjectType.Building;
                            targetId = b.AutoID;
							immediateCreate = RolePermissionCheck.CheckCreateViewOrder(request.UserInfo.Session, targetUser, b);
                        }
                        else
                        {
                            throw new FileNotFoundException("Property not found in system.");
                        }
                    }
                }
                else
                {
                    // view by property ID
                    //
                    UpdateableBase to = null;
					targetId = request.Request.GetPropertyId();

					switch (request.Request.GetPropertyType())
					{
						case ViewOrder.SubjectType.Suite:
							{
								targetType = ViewOrder.SubjectType.Suite;
								Suite s;
								using (SuiteDao dao = new SuiteDao(request.UserInfo.Session.DbSession)) s = dao.GetById(targetId);
								// The following is not applicable any more: multiple VOs per object may co-exist; suite status does not matter
								//if (s.Status != Suite.SalesStatus.Sold) throw new ObjectExistsException("Suite status in not SOLD");
								to = s;
								immediateCreate = RolePermissionCheck.CheckCreateViewOrder(request.UserInfo.Session, targetUser, s.Building);
								break;
							}

						case ViewOrder.SubjectType.Building:
							{
								targetType = ViewOrder.SubjectType.Building;
								Building b;
								using (BuildingDao dao = new BuildingDao(request.UserInfo.Session.DbSession)) b = dao.GetById(targetId);
								// The following is not applicable any more: multiple VOs per object may co-exist; building status does not matter
								//if (product != ViewOrder.ViewOrderProduct.Building3DLayout)
								//{
								//    if (b.Status != Building.BuildingStatus.Sold) throw new ObjectExistsException("Building status in not SOLD");
								//}
								to = b;
								immediateCreate = RolePermissionCheck.CheckCreateViewOrder(request.UserInfo.Session, targetUser, b);
								break;
							}

						default:
							throw new ArgumentException("Unknown property type");
					}

                    if (null == to) throw new FileNotFoundException("Property not found in system.");
                }

				if (immediateCreate)
				{
					ReverseRequestService.CreateViewOrder(request, targetUser, note,
						product, options, mlsId, mlsUrl, targetType, targetId, productUrl, expiresOn, paymentRefId);
				}
				else
				{
					TimeSpan unpaidViewOrderLifespan = new TimeSpan(0, 
						Configuration.PaymentSystem.UnpaidViewOrderLifespanMinutes.Value, 
						0);

					// TODO: verify amount to be paid

					var vo = ReverseRequestService.CreateViewOrder(request, targetUser, note,
						product, options, mlsId, mlsUrl, targetType, targetId, productUrl, 
						DateTime.UtcNow.Add(unpaidViewOrderLifespan), null);

					var rr = ReverseRequest.CreateViewOrderActivation(vo, netAmountToPay, expiresOn);
					request.UserInfo.Session.DbSession.Save(rr);

					ServiceInstances.MessageGen.SendMessage(
						ServiceInstances.EmailSender, anonymousOwnerEmail,
						"MSG_PAYMENT_REQUEST", 
						vo.Product,
						vo.Options,
						NotifyExpiringViewOrders.GetSubjectAddress(request.UserInfo.Session.DbSession, vo),
						ReverseRequestService.GenerateUrl(vo),
						vo.ExpiresOn.ToLocalTime(), // LEGACY: SERVER LOCAL TIME HERE!
						netAmountToPay.ToFullString(),
						ReverseRequestService.GenerateUrl(rr),
						expiresOn.ToLocalTime(), // LEGACY: SERVER LOCAL TIME HERE!
						mlsId ?? string.Empty,
						mlsUrl ?? string.Empty, string.IsNullOrEmpty(mlsUrl) ? "@N/A" : mlsUrl,
						productUrl ?? string.Empty, string.IsNullOrEmpty(productUrl) ? "@N/A" : productUrl
						);

				}

                // request.Response.ResponseCode - set by .CreateListing()
                tran.Commit();
            }
        }

		private static DateTime getViewOrderTimeArgument(IServiceRequest request)
		{
			DateTime expiresOn;
			string dv = request.Request.Query["daysvalid"];
			if (string.IsNullOrEmpty(dv)) dv = request.Request.Query["daysValid"];  // OBSOLETE URI
			if (string.IsNullOrWhiteSpace(dv))
			{
				dv = request.Request.Query["expireson"];
				if (string.IsNullOrEmpty(dv)) dv = request.Request.Query["expiresOn"];  // OBSOLETE URI
				if (string.IsNullOrWhiteSpace(dv)) throw new ArgumentException("Missing validation period/expiry");

				if (!DateTime.TryParseExact(dv, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out expiresOn))
					throw new ArgumentException("Validation expiry value is invalid");

				if (expiresOn.CompareTo(DateTime.UtcNow) < 1) throw new ArgumentException("Validation expiry value is too old");
			}
			else
			{
				int idv;
				if (!int.TryParse(dv, out idv)) throw new ArgumentException("Validation period value is invalid");
				// LEGACY: SERVER LOCAL TIME HERE!
				expiresOn = DateTime.Now.Date.AddDays(idv + 1).ToUniversalTime();
			}
			return expiresOn;
		}

		private static void registerBuyingAgentViewOrder(IServiceRequest request,
			string mlsId, string paymentRefId)
		{
			ISession dbSession = request.UserInfo.Session.DbSession;

			using (var tran = NHibernateHelper.OpenNonNestedTransaction(dbSession))
			{
				ViewOrder.SubjectType targetType;
				int targetId;

				retrieveExistingViewOrder(request, ref mlsId, true, dbSession, out targetType, out targetId);

				ViewOrder newVo = ReverseRequestService.CreateViewOrder(request, 
					request.UserInfo.Session.User, 
					null,
					ViewOrder.ViewOrderProduct.PrivateListing,
					ViewOrder.ViewOrderOptions.VirtualTour3D,  // TODO: Should be configurable?
					mlsId, null, targetType, targetId,
					null, 
					DateTime.UtcNow.AddYears(1), // the period is until property is sold or up to one year - whichever comes first
					paymentRefId);

				newVo.InfoUrl = string.Format("{0}info.html#{1}!{2}",
					request.Request.ConstructClientRootUri(),
					"mls",
					UniversalId.GenerateUrlId(UniversalId.IdType.ViewOrder, newVo.AutoID));

				using (var dao = new ViewOrderDao(request.UserInfo.Session.DbSession))
					dao.Update(newVo);

				tran.Commit();
				ServiceInstances.Logger.Info("User {0} created a BuyingAgent-type ViewOrder: {1},{2}, MLS#{3}, VOID:{4}, PR:{5}",
					request.UserInfo.Session.User,
					targetType, targetId, mlsId, newVo.AutoID, paymentRefId);
			}
		}

		private static void registerAgentViewOrder(IServiceRequest request,
			string mlsId, string paymentRefId)
		{
			ISession dbSession = request.UserInfo.Session.DbSession;

			using (var tran = NHibernateHelper.OpenNonNestedTransaction(dbSession))
			{
				ViewOrder.SubjectType targetType;
				int targetId;

				retrieveExistingViewOrder(request, ref mlsId, false, dbSession, out targetType, out targetId);

				ViewOrder newVo = ReverseRequestService.CreateViewOrder(request,
					request.UserInfo.Session.User,
					null,
					request.Request.HasProductType() ? 
						request.Request.GetProductType() : ViewOrder.ViewOrderProduct.PrivateListing,
					request.Request.HasProductOptions() ?
						request.Request.GetProductOptions() : ViewOrder.ViewOrderOptions.VirtualTour3D,  // TODO: Should be configurable?
					mlsId, null, targetType, targetId,
					null,
					DateTime.UtcNow.AddYears(1), // the period is until property is sold or up to one year - whichever comes first
					paymentRefId);

				newVo.InfoUrl = string.Format("{0}info.html#{1}!{2}",
					request.Request.ConstructClientRootUri(),
					"mls",
					UniversalId.GenerateUrlId(UniversalId.IdType.ViewOrder, newVo.AutoID));

				using (var dao = new ViewOrderDao(request.UserInfo.Session.DbSession))
					dao.Update(newVo);

				Money? price = null;
				if (request.Request.HasPrice())
				{
					price = request.Request.GetPrice();
					var target = newVo.GetTarget(request.UserInfo.Session.DbSession);

					Suite suite = target as Suite;
					if ((suite != null) && price.HasValue)
					{
						suite.CurrentPrice = price;
						using (var dao = new SuiteDao(request.UserInfo.Session.DbSession))
							dao.SafeUpdate(suite);
						using (var man = new SiteManager(request.UserInfo.Session))
							man.LogNewSuitePrice(suite, (float)Convert.ToDouble(suite.CurrentPrice));
					}
				}

				tran.Commit();

				ServiceInstances.Logger.Info("User {0} created an Agent-type ViewOrder: {1},{2}, MLS#{3}, price:{4}, VOID:{5}, PR:{6}",
					request.UserInfo.Session.User,
					targetType, targetId, 
					string.IsNullOrEmpty(mlsId) ? "N/A" : mlsId,
					price.HasValue ? price.Value.ToFullString() : "<unchanged>",
					newVo.AutoID, paymentRefId);
			}
		}

		private static void retrieveExistingViewOrder(IServiceRequest request,
			ref string mlsId, bool requireMlsId,
			ISession dbSession, 
			out ViewOrder.SubjectType targetType, out int targetId)
		{
			string voIdStr = request.Request.Query["vieworderid"];
			if (string.IsNullOrEmpty(voIdStr)) voIdStr = request.Request.Query["voId"];  // OBSOLETE URI
			Guid voId = Guid.Empty;
			if (!string.IsNullOrEmpty(voIdStr))
			{
				switch (UniversalId.TypeInUrlId(voIdStr))
				{
					case UniversalId.IdType.ViewOrder:
					case UniversalId.IdType.Unknown:
						voId = UniversalId.ExtractAsGuid(voIdStr);
						break;

					default:
						throw new ArgumentException("ViewOrder ID provided format is unknown.");
				}
			}

			// If ViewOrder ID is provided in request - use it...
			if (voId != Guid.Empty)
			{
				ViewOrder vo;
				using (var dao = new ViewOrderDao(dbSession))
					vo = dao.GetById(voId);

				if (null == vo)
					throw new FileNotFoundException("ViewOrder referenced by ID is not found");

				mlsId = vo.MlsId;
				targetType = vo.TargetObjectType;
				targetId = vo.TargetObjectId;

				if (string.IsNullOrEmpty(mlsId))
					throw new ArgumentException("ViewOrder referenced by ID has no MLS# associated");

				return;
			}
			// ... if not - try MLS# provided ...
			else if (!string.IsNullOrEmpty(mlsId))
			{
				bool found = false;
				using (var dao = new ViewOrderDao(dbSession))
				{
					foreach (var vo in dao.GetByImportedMlsId(mlsId))
					{
						targetType = vo.TargetObjectType;
						targetId = vo.TargetObjectId;
						found = true;

						if (request.Request.HasPropertyType())
						{
							if ((targetType == request.Request.GetPropertyType())
								|| (targetId == request.Request.GetPropertyId()))
								return;
						}
						else
						{
							// TODO: May need to find the Resale-bound ViewOrder here!
							return;
						}
					}
				}
				if (found && request.Request.HasPropertyType())
					// we found something; the request has property reference and those two do not match
					throw new ArgumentException("The MLS number provided does not refer to this property");
			}
			// ... else - try searching by property ID and type provided
			targetType = request.Request.GetPropertyType();
			targetId = request.Request.GetPropertyId();

			if (string.IsNullOrEmpty(mlsId))
			{
				mlsId = findMlsIdForTarget(dbSession, targetType, targetId);

				if ((null == mlsId) && requireMlsId)
					throw new FileNotFoundException("No known View Order with defined MLS ID exists for the target.");

				checkTargetExists(dbSession, targetType, targetId);
			}
			else // MLS# is provided and is not known to the system
			{
				checkTargetExists(dbSession, targetType, targetId);
				createMlsInfoStubRecord(dbSession, mlsId);
			}
		}

		private static string findMlsIdForTarget(ISession dbSession, ViewOrder.SubjectType targetType, int targetId)
		{
			string result = null;

			// The only link between targets and MLS records are ViewOrders
			// At least one imported must exist.

			using (var dao = new ViewOrderDao(dbSession))
				foreach (var vo in dao.GetActive(targetType, targetId))
					if (!string.IsNullOrEmpty(vo.MlsId)) { result = vo.MlsId; break; }
			
			return result;
		}

		private static void createMlsInfoStubRecord(ISession dbSession, string mlsId)
		{
			Mls.MlsItem mlsSource = new Mls.MlsItem();
			mlsSource.MlsId = mlsId;
			using (var dao = new MlsInfoDao(dbSession)) dao.Create(new MlsInfo(mlsSource));
		}

		private static void checkTargetExists(ISession dbSession, ViewOrder.SubjectType targetType, int targetId)
		{
			switch (targetType)
			{
				case ViewOrder.SubjectType.Building:
					{
						Building b;
						using (var dao = new BuildingDao(dbSession)) b = dao.GetById(targetId);
						if (null == b) throw new FileNotFoundException("Property type/ID not found.");
					}
					break;

				case ViewOrder.SubjectType.Suite:
					{
						Suite s;
						using (var dao = new SuiteDao(dbSession)) s = dao.GetById(targetId);
						if (null == s) throw new FileNotFoundException("Property type/ID not found.");
					}
					break;
			}
		}

        private static void checkAddress(IServiceRequest request)
        {
            UpdateableBase to = AddressHelper.ParseGeographicalAddressToModel(request.Request.Query, request.UserInfo.Session.DbSession);
            ClientData result = new ClientData();

            if (null == to)
            {
                result.Add("result", false);
            }
            else
            {
                // Only suite is currently supported
                Suite s = to as Suite;
                if (null == s) throw new NotImplementedException();

                result.Add("result", true);

                string readable = AddressHelper.ConvertToReadableAddress(s.Building, s);
                ClientData parsed = AddressHelper.ConvertToNormalizedAddress(s.Building, s);

                result.Add("normalizedAddress", parsed);
                result.Add("readableAddress", readable);
                result.Add("propertyType", "suite");
                result.Add("propertyId", s.AutoID);
            }

            request.Response.Data = result;
            request.Response.ResponseCode = HttpStatusCode.OK;
        }

        private static void registerUser(IServiceRequest request)
        {
            // This call is for Selling Agent registrations only. All other user registrations should go through data service.
            string login = request.Request.Query["email"];
            ReverseRequestService.InitiateUserRegistration(login);
            request.Response.ResponseCode = HttpStatusCode.OK;
        }

		private static void recoverPassword(IServiceRequest request)
		{
			ReverseRequestService.InitiatePasswordRecover(
				request.Request.Query.GetParam("role", string.Empty),
				request.Request.Query.GetParam("ed", string.Empty),
				request.Request.Query.GetParam("uid", string.Empty));
			request.Response.ResponseCode = HttpStatusCode.OK;
		}

        private static LoginType parseLoginType(ServiceQuery args)
        {
            string type = args.GetParam("type", "plain").ToLower();
            if (type.Equals("plain")) return LoginType.Plain;
            // ...
            return LoginType.Plain;
        }

		private static void getMlsInfo(IServiceRequest request)
		{
			var strObjectId = request.Request.Query.GetParam("id", string.Empty);
			if (string.IsNullOrWhiteSpace(strObjectId)) 
				throw new ArgumentException("Subject ID missing or invalid");

			using (var session = NHibernateHelper.GetSession())
			{
				var resp = request.Response;
				using (var tran = NHibernateHelper.OpenNonNestedTransaction(session))
				{
					var viewOrder = DataService.RetrieveViewOrder(session, strObjectId, false);

					resp.Data = DataService.convertViewOrderdata(session, 
						viewOrder, request.Request.Query.GetParam("verbose", false));

					resp.Data = new ClientData();
					resp.Data.Add("voId", viewOrder.AutoID);
					resp.Data.Add("mlsId", viewOrder.MlsId);

					MlsInfo extraInfo;
					using (var dao = new MlsInfoDao(session))
						extraInfo = dao.GetByMlsNum(viewOrder.MlsId);

					// TODO: converting text->json->text
					if (extraInfo != null)
						resp.Data.Add("data", extraInfo.RawInfoAsClientData);

					User u;
					using (var dao = new UserDao(session))
						u = dao.GetById(viewOrder.OwnerId);

					resp.Data.Add("brokerInfo", u.PersonalInfo);
					if (u.BrokerInfo != null) resp.Data.Add("brokerage", u.BrokerInfo.GetClientData());
				}
				resp.ResponseCode = HttpStatusCode.OK;
			}
		}

		private static void storeTempFile(IServiceRequest request)
		{
			ensureSessionExists(request);

			if (request.Request.RawDataContentType != null)
			{
				var result = storeTempFile(request.UserInfo.Session, 
					request.Request.RawDataContentType,
					new MemoryStream(request.Request.RawData));

				request.Response.ResponseCode = HttpStatusCode.OK;
				request.Response.Data = new ClientData();
				request.Response.Data.Add("url", result);
			}
			else if (request.Request.FormData != null)
			{
				bool found = false;
				foreach (var item in request.Request.FormData.Items)
				{
					if (item.ContentType != null)
					{
						var result = storeTempFile(
							request.UserInfo.Session, 
							item.ContentType, item.Data);

						request.Response.ResponseCode = HttpStatusCode.OK;
						request.Response.DataStreamContentType = "txt";

						var bytes = Encoding.ASCII.GetBytes(result);
						request.Response.DataStream.Write(bytes, 0, bytes.Length);

						found = true;
						break;
					}
				}
				if (!found) throw new ArgumentException("Request does not contain a file (1)");
			}
			else
			{
				throw new ArgumentException("Request does not contain a file (0)");
			}
		}

		private static string storeTempFile(ClientSession session, 
			string contentType, Stream data)
		{
			string extension = "bin";
			// TODO: SLOW SEARCH (Low Impact)
			foreach (var kvp in HttpServiceBase.ContentTypeByExtension)
			{
				if (kvp.Value.Equals(contentType)) { extension = kvp.Key; break; }
			}

			return session.AddTempFile(extension, data);
		}

		private static void setUserPhoto(IServiceRequest request)
		{
			ensureSessionExists(request);
			
			var q = request.Request.Query;
			var url = q.GetParam("photourl", string.Empty);
			var topCut = q.GetParam("pc_toppx", 0);
			var leftCut = q.GetParam("pc_leftpx", 0);
			var rightCut = q.GetParam("pc_rightpx", 0);
			var bottomCut = q.GetParam("pc_bottompx", 0);

			if (string.IsNullOrEmpty(url)) throw new ArgumentException("Required parameter missing: photourl");

			string result = null;
			try
			{
				var session = request.UserInfo.Session;
				var path = ServiceInstances.ImmediateFileStorageManager.ConvertToRelativePath(url);
				if (string.IsNullOrEmpty(path))
				{
					throw new ArgumentException("Only hosted image URLs are allowed");
				}
				else
				{
					using (var s = ServiceInstances.ImmediateFileStorageManager.OpenFile(path))
					{
						result = setUserPhoto(session,
							s, topCut, leftCut, rightCut, bottomCut);
					}
				}

				using (var tran = NHibernateHelper.OpenNonNestedTransaction(session))
				{
					session.DbSession.Refresh(session.User);  // is this required?
					session.User.PhotoUrl = result;
					session.DbSession.Update(session.User);

					tran.Commit();
				}
			}
			catch
			{
				if (result != null)
					ServiceInstances.FileStorageManager.RemoveFile(result);
				throw;
			}

			request.Response.ResponseCode = HttpStatusCode.OK;
			request.Response.Data = new ClientData();
			request.Response.Data.Add("url",
				ServiceInstances.FileStorageManager.ConvertToFullPath(result));
		}

		private static string setUserPhoto(ClientSession session,
			Stream photo, int topCut, int leftCut, int rightCut, int bottomCut)
		{
			var img = ImageProcessing.ConditionImage(new Bitmap(photo),
				leftCut, topCut, rightCut, bottomCut,
				Configuration.User.UserPhotoWidthPx.Value,
				Configuration.User.UserPhotoHeightPx.Value);

			using (var s = new MemoryStream())
			{
				ImageProcessing.SaveJpeg(s, img, 80);

				s.Position = 0L;

				return ServiceInstances.FileStorageManager.StoreFile("user", "photo",
					"jpeg", null, s);
			}
		}

        private static void credit(IServiceRequest request)
        {
            if ((null == request.UserInfo.Session)
                || !request.UserInfo.Session.TrustedConnection
                || (request.UserInfo.Session.User.UserRole != User.Role.SuperAdmin))
                throw new ArgumentException("Program command not understood.");  // conceal request availability

            var targetType = request.Request.Query.GetParam("targettype", string.Empty);
            var targetId = request.Request.Query.GetParam("targetid", -1);
            decimal amount;

            if (targetId < 0) throw new ArgumentException("Target (creditee) ID not provided or is invalid");
            
            if (!decimal.TryParse(request.Request.Query.GetParam("amount", "0"), out amount))
                throw new ArgumentException("Credit amount not provided or is invalid");

            if (targetType.Equals("user", StringComparison.InvariantCultureIgnoreCase))
            {
                using (var man = new UserManager(request.UserInfo.Session))
                    amount = man.Credit(targetId, amount);

                request.Response.ResponseCode = HttpStatusCode.OK;
                request.Response.Data = new ClientData();
                request.Response.Data.Add("userId", targetId);
                request.Response.Data.Add("creditUnits", amount);
            }
            else if (targetType.Equals("brokerage", StringComparison.InvariantCultureIgnoreCase))
            {
                using (var man = new BrokerageManager(request.UserInfo.Session))
                    amount = man.Credit(targetId, amount);

                request.Response.ResponseCode = HttpStatusCode.OK;
                request.Response.Data = new ClientData();
                request.Response.Data.Add("brokerageId", targetId);
                request.Response.Data.Add("creditUnits", amount);
            }
            else throw new ArgumentException("Target type (creditee) not provided or is invalid");
        }
    }
}