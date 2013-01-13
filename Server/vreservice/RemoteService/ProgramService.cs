using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.HttpService;
using System.Web;
using System.IO;
using Vre.Server.Dao;

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
                if (command.Equals("login"))
                {
                    login(request);
                    return;
                }
                else if (command.Equals("sessionrenew"))
                {
                    sessionRenew(request);
                    return;
                }
                else if (command.Equals("chpwd"))
                {
                    changePassword(request);
                    return;
                }
                else if (command.Equals("chlogin"))
                {
                    changeLogin(request);
                    return;
                }
                else if (command.Equals("grantaccess"))
                {
                    grantAccess(request);
                    return;
                }
                else if (command.Equals("license"))
                {
                    licenseUser(request);
                    return;
                }
                else if (command.Equals("assignseller"))
                {
                    assignSeller(request);
                    return;
                }
                else if (command.Equals("register"))
                {
                    register(request);
                    return;
                }
                else if (command.Equals("check"))
                {
                    check(request);
                    return;
                }
            }

            throw new ArgumentException("Program command not understood.");
        }

        private static void login(IServiceRequest request)
        {
            // parse required arguments
            //
            LoginType loginType = parseLoginType(request.Request.Query);
            User.Role role;
            int estateDeveloperId;
            string login = request.Request.Query["uid"];
            string password = request.Request.Query["pwd"];
            string sessionId = null;
            if (!Enum.TryParse<User.Role>(request.Request.Query["role"], true, out role)) role = User.Role.SuperAdmin;
            // TODO: Add login by Estate Developer Alias
            if (!int.TryParse(request.Request.Query["ed"], out estateDeveloperId)) estateDeveloperId = -1;

            // authenticate
            //
            if ((!string.IsNullOrWhiteSpace(login)) && (!string.IsNullOrWhiteSpace(password)))
            {
                sessionId = ServiceInstances.SessionStore.LoginUser(request.UserInfo.EndPoint, 
                    loginType, role, estateDeveloperId, login, password);
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
                request.Response.ResponseCode = HttpStatusCode.OK;
                request.Response.Data = new ClientData();
                request.Response.Data.Add("sid", sessionId);
                request.Response.Data.Add("keepalivePeriodSec", ServiceInstances.SessionStore.ClientKeepalivePeriodSec);
                request.Response.Data.Add("userId", ServiceInstances.SessionStore[sessionId].User.AutoID);
            }
            else
            {
                request.Response.ResponseCode = HttpStatusCode.Forbidden;
                request.Response.ResponseCodeDescription = "Invalid logn type, login name or password.";
            }
        }

        private static void changePassword(IServiceRequest request)
        {
            // parse required arguments
            //
            LoginType loginType = parseLoginType(request.Request.Query);
            string login = request.Request.Query["uid"];
            User.Role role;
            int estateDeveloperId;
            string password = request.Request.Query["pwd"];
            string newPassword = request.Request.Query["npwd"];
            if (!Enum.TryParse<User.Role>(request.Request.Query["role"], true, out role)) role = User.Role.Visitor;
            // TODO: Add login by Estate Developer Alias
            if (!int.TryParse(request.Request.Query["ed"], out estateDeveloperId)) estateDeveloperId = -1;

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
                        manager.ChangePassword(loginType, role, estateDeveloperId, login, password, newPassword);
                    }
                }
            }

            request.Response.ResponseCode = HttpStatusCode.OK;
        }

        private static void changeLogin(IServiceRequest request)
        {
            string newLogin = request.Request.Query["newLogin"];
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

            if (entity.Equals("viewOrder"))
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
            ViewOrder.ViewOrderType product;
            string paymentRefId = request.Request.Query["pr"];
            string productUrl = request.Request.Query["evt_url"];
            string mlsId = request.Request.Query["mls_id"];
            string mlsUrl = request.Request.Query["mls_url"];
            string note = request.Request.Query["note"];
            string propertyType = request.Request.Query["propertyType"];
            string propertyId = request.Request.Query["propertyId"];
            string ownerId = request.Request.Query["ownerId"];
            ViewOrder.SubjectType targetType;
            int targetId;

            {
                string dv = request.Request.Query["daysValid"];
                if (string.IsNullOrWhiteSpace(dv))
                {
                    dv = request.Request.Query["expiresOn"];
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
            }
            //if (!DateTime.TryParseExact(request.Request.Query["expires"], "yyyy-MM-ddTHH:mm:ss",
            //    CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out expiresOn))
            //{
            //    throw new ArgumentException("The time specified is not valid");
            //}

            string pt = request.Request.Query["product"];
            if (string.IsNullOrWhiteSpace(pt)) throw new ArgumentException("Product type missing");
            if (pt.Equals("fp")) product = ViewOrder.ViewOrderType.FloorPlan;
            else if (pt.Equals("evt")) product = ViewOrder.ViewOrderType.ExternalTour;
            else if (pt.Equals("3dt")) product = ViewOrder.ViewOrderType.VirtualTour3D;
            else throw new ArgumentException("Product type is unknown");

            //if (string.IsNullOrWhiteSpace(paymentRefId)) throw new ArgumentException("Required parameter missing");

            if (product == ViewOrder.ViewOrderType.ExternalTour)
            {
                if (string.IsNullOrWhiteSpace(productUrl))
                    throw new ArgumentException("External Virtual Tour reference not provided");
                productUrl = HttpUtility.UrlDecode(productUrl);
            }

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(request.UserInfo.Session.DbSession))
            {
                if (string.IsNullOrWhiteSpace(propertyType) || string.IsNullOrWhiteSpace(propertyId))
                {
                    // view by address lookup
                    //
                    UpdateableBase to = AddressHelper.ParseGeographicalAddressToModel(request.Request.Query, request.UserInfo.Session.DbSession);

                    Suite s = to as Suite;
                    if (s != null)
                    {
                        targetType = ViewOrder.SubjectType.Suite;
                        targetId = s.AutoID;
                    }
                    else
                    {
                        Building b = to as Building;
                        if (b != null)
                        {
                            targetType = ViewOrder.SubjectType.Building;
                            targetId = b.AutoID;
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
                    if (!int.TryParse(propertyId, out targetId)) throw new ArgumentException("Property ID is not valid");

                    UpdateableBase to = null;

                    if (propertyType.Equals("suite"))
                    {
                        targetType = ViewOrder.SubjectType.Suite;
                        Suite s;
                        using (SuiteDao dao = new SuiteDao(request.UserInfo.Session.DbSession)) s = dao.GetById(targetId);
                        if (s.Status != Suite.SalesStatus.Sold) throw new ObjectExistsException("Suite status in not SOLD");
                        to = s;
                    }
                    else if (propertyType.Equals("building"))
                    {
                        targetType = ViewOrder.SubjectType.Building;
                        Building b;
                        using (BuildingDao dao = new BuildingDao(request.UserInfo.Session.DbSession)) b = dao.GetById(targetId);
                        if (b.Status != Building.BuildingStatus.Sold) throw new ObjectExistsException("Building status in not SOLD");
                        to = b;
                    }
                    else throw new ArgumentException("Unknown property type");

                    if (null == to) throw new FileNotFoundException("Property not found in system.");
                }

                // use override for view order owner
                int userId = -1;
                if (!string.IsNullOrWhiteSpace(ownerId))
                {
                    if (!int.TryParse(ownerId, out userId)) userId = -1;
                }

                string viewOrderId = ReverseRequestService.CreateViewOrder(request, userId, note,
                    product, mlsId, mlsUrl, targetType, targetId, productUrl, expiresOn, paymentRefId);

                // request.Response.ResponseCode - set by .CreateListing()
                tran.Commit();
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
            string login = request.Request.Query["email"];
            ReverseRequestService.InitiateUserRegistration(request.Request, request.UserInfo.Session, login);
            request.Response.ResponseCode = HttpStatusCode.OK;
        }

        private static LoginType parseLoginType(ServiceQuery args)
        {
            string type = args.GetParam("type", "plain").ToLower();
            if (type.Equals("plain")) return LoginType.Plain;
            // ...
            return LoginType.Plain;
        }
    }
}