using System.Net;
using Vre.Server.RemoteService;

namespace Vre.Server.HttpService
{
    internal class HttpServiceMain : HttpServiceBase
    {
        private RemoteServiceProvider _rsp;

        public HttpServiceMain() : base("HTTP Listener")
        {
            _rsp = new RemoteServiceProvider();
        }

        protected override IResponseData process(string browserKey, HttpListenerContext ctx)
        {
            HttpServiceRequest rq = new HttpServiceRequest(ctx, _path);

            if (_allowExtendedLogging)
            {
                ClientSession cs = rq.UserInfo.Session;
                if (cs != null)
                    ServiceInstances.RequestLogger.Info("Session={0}; BK={1}; {2}; URL={3}", cs, browserKey, ctx.Request.HttpMethod, ctx.Request.Url);
                else
                    ServiceInstances.RequestLogger.Info("Anonymous; BK={0}; {1}; URL={2}", browserKey, ctx.Request.HttpMethod, ctx.Request.Url);
            }

            if (!rq.UserInfo.StaleSession)
            {
                try
                {
                    if (rq.UserInfo.Session != null) rq.UserInfo.Session.Resume();
                    _rsp.ProcessRequest(this, rq);
                }
                finally
                {
                    if (rq.UserInfo.Session != null) rq.UserInfo.Session.Disconnect();
                }
            }

            return rq.Response;
        }
    }
}