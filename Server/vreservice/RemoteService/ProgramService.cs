using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.HttpService;
using System.Web;

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
            string mslId = request.Request.Query["msl_id"];
            string propertyType = request.Request.Query["propertyType"];
            string propertyId = request.Request.Query["propertyId"];
            string ownerId = request.Request.Query["ownerId"];
            int suiteId;

            {
                string dv = request.Request.Query["daysValid"];
                if (string.IsNullOrWhiteSpace(dv)) throw new ArgumentException("Missing validation period");
                int idv;
                if (!int.TryParse(dv, out idv)) throw new ArgumentException("Validation period value is invalid");
                // TODO: LOCAL TIME HERE!
                expiresOn = DateTime.Now.AddDays(idv).ToUniversalTime().Date;
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

            if (string.IsNullOrWhiteSpace(mslId))
            {
                if (string.IsNullOrWhiteSpace(propertyType) || string.IsNullOrWhiteSpace(propertyId))
                {
                    // view by address lookup
                    //
                    List<UpdateableBase> to = AddressHelper.ParseGeographicalAddressToModel(request.Request.Query, request.UserInfo.Session.DbSession);

                    // Only unique result is possible here
                    if ((null == to) || (to.Count < 1)) throw new ArgumentException("Address not found");
                    if (to.Count > 1) throw new ArgumentException("Address: non-unique result returned; please add details");

                    // Only suite is currently supported
                    Suite s = to[0] as Suite;
                    if (null == s) throw new NotImplementedException();

                    suiteId = s.AutoID;
                }
                else
                {
                    // view by property ID
                    //
                    if (!propertyType.Equals("suite")) throw new ArgumentException("Unknown property type");
                    if (!int.TryParse(propertyId, out suiteId)) throw new ArgumentException("Property ID is not valid");

                    // TODO: Verify that property ID exists?
                }
            }
            else
            {
                // TODO: Test against MLS DB
                //
                throw new NotImplementedException();
            }

            // use override for view order owner
            int userId = -1;
            if (!string.IsNullOrWhiteSpace(ownerId))
            {
                if (!int.TryParse(ownerId, out userId)) userId = -1;
            }

            string viewOrderId = ReverseRequestService.CreateViewOrder(request, userId,
                product, mslId, ViewOrder.SubjectType.Suite, suiteId, productUrl, expiresOn, paymentRefId);

            // request.Response.ResponseCode - set by .CreateListing()
        }

        private static void checkAddress(IServiceRequest request)
        {
            List<UpdateableBase> to = AddressHelper.ParseGeographicalAddressToModel(request.Request.Query, request.UserInfo.Session.DbSession);
            ClientData result = new ClientData();

            if ((null == to) || (to.Count < 1)) 
            {
                result.Add("result", false);
            }
            else if (to.Count > 1)
            {
                throw new ArgumentException("Address: non-unique result returned; please add details");
            }
            else
            {
                // Only suite is currently supported
                Suite s = to[0] as Suite;
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