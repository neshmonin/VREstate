using System.Net;
using Vre.Server.RemoteService;

namespace Vre.Server.HttpService
{
    internal class HttpServiceMain : HttpServiceBase
    {
        private RemoteServiceProvider _rsp;
        private long _maxRequestBodyLength;

        public HttpServiceMain() : base("HTTP Listener")
        {
            _rsp = new RemoteServiceProvider();
            _maxRequestBodyLength = ServiceInstances.Configuration.GetValue("MaxHttpRequestBodyLengthBytes", 10240);
        }

        protected override IResponseData process(string browserKey, HttpListenerContext ctx, HttpServiceRequest.ProcessResponse proc)
        {
            HttpServiceRequest rq = new HttpServiceRequest(ctx, _path, _maxRequestBodyLength, proc);

            if (_allowExtendedLogging) ServiceInstances.RequestLogger.Info(prepareCallerInfo(browserKey, ctx, rq));

            if (!rq.UserInfo.StaleSession)
            {
                if (rq.UserInfo.Session != null)
                {
                    lock (rq.UserInfo.Session)
                    {
                        try
                        {
                            rq.UserInfo.Session.Resume();
                            _rsp.ProcessRequest(this, rq);
                            rq.UserInfo.Session.Disconnect(false);
                        }
                        catch (NHibernate.HibernateException)
                        {
                            rq.UserInfo.Session.Disconnect(true);
                            throw;
                        }
                        catch
                        {
                            rq.UserInfo.Session.Disconnect(false);
                            throw;
                        }
                    }
                }
                else
                {
                    _rsp.ProcessRequest(this, rq);
                }
            }

            return rq.Response;
        }
    }
}