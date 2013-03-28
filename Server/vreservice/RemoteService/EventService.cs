using System;
using Vre.Server.BusinessLogic;
using Vre.Server.RemoteService;

namespace Vre.Server.HttpService
{
    internal class EventService : IHttpService
    {
        private static bool _configured = false;
        private static bool _allowUnsecureService = true;

        private const string _servicePathPrefix = _servicePathElement0;
        private const string _servicePathElement0 = "ev";

        public string ServicePathPrefix { get { return _servicePathPrefix; } }

        public bool RequiresSession { get { return true; } }

        private static void configure()
        {
            _allowUnsecureService = ServiceInstances.Configuration.GetValue("AllowSensitiveDataOverNonSecureConnection", false);

            _configured = true;
        }

        #region unused service types
        public void ProcessCreateRequest(IServiceRequest request)
        {
            throw new NotImplementedException();
        }

        public void ProcessReplaceRequest(IServiceRequest request)
        {
            throw new NotImplementedException();
        }

        public void ProcessDeleteRequest(IServiceRequest request)
        {
            throw new NotImplementedException();
        }
        #endregion

        public void ProcessGetRequest(IServiceRequest request)
        {
            if (!_configured) configure();

            if (!_allowUnsecureService && !request.Request.IsSecureConnection)
                throw new PermissionException("Service available only over secure connection.");

            request.UserInfo.Session.StartEventThread(request.Response);
            request.Response.HoldResponseForServerPush = true;
        }
    }
}