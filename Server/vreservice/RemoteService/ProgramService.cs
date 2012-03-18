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
            string login = request.Request.Query["uid"];
            string password = request.Request.Query["pwd"];
            string sessionId = null;

            // authenticate
            //
            if ((!string.IsNullOrWhiteSpace(login)) && (!string.IsNullOrWhiteSpace(password)))
            {
                sessionId = ServiceInstances.SessionStore.LoginUser(request.UserInfo.EndPoint, loginType, login, password);
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
            string password = request.Request.Query["pwd"];
            string newPassword = request.Request.Query["npwd"];

            // authenticate
            //
            using (ISession session = NHibernateHelper.GetSession())
            {
                using (IAuthentication auth = new Authentication(session))
                {
                    string errorReason;
                    if (auth.ChangePassword(loginType, login, password, newPassword, out errorReason))
                    {
                        request.Response.ResponseCode = HttpStatusCode.OK;
                        request.Response.Data = new ClientData();
                        request.Response.Data.Add("updated", 1);
                    }
                    else
                    {
                        throw new InvalidOperationException(errorReason);
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