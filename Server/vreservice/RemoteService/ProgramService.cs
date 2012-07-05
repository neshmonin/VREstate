using System;
using System.Net;
using Vre.Server.BusinessLogic;
using NHibernate;

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
            if (role == User.Role.SellingAgent)
            {
                ClientSession cs = ServiceInstances.SessionStore[sessionId];
                if (!cs.User.HasAnyLicense())
                {
                    ServiceInstances.Logger.Info("User {0} has no valid licenses; rejecting login.", cs);
                    cs = null;
                    ServiceInstances.SessionStore.DropSession(sessionId);
                    sessionId = null;
                }
            }

            // produce output
            //
            if (sessionId != null)
            {
                request.Response.ResponseCode = HttpStatusCode.OK;
                request.Response.Data = new ClientData();
                request.Response.Data.Add("sid", sessionId);
                request.Response.Data.Add("keepalivePeriodSec", ServiceInstances.SessionStore.ClientKeepalivePeriodSec);
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

            // authenticate
            //
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

        private static LoginType parseLoginType(ServiceQuery args)
        {
            string type = args.GetParam("type", "plain").ToLower();
            if (type.Equals("plain")) return LoginType.Plain;
            // ...
            return LoginType.Plain;
        }
    }
}