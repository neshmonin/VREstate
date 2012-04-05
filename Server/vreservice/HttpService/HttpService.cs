using System;
using System.Collections.Generic;
using System.Net;
using Vre.Server.RemoteService;
using System.Threading;

namespace Vre.Server.HttpService
{
    public class HttpServiceManager
    {
        private static readonly object _lock = new object();
        private static List<string> _listeners = new List<string>();

        public static string Status = "Stopped.";
        public static string[] Listeners { get { lock (_listeners) return _listeners.ToArray(); } }

        private static HttpListener _httpListener;

        private static RemoteServiceProvider _rsp;
        private static string _path;

        private static bool _allowExtendedLogging;

        public static void PerformStartup()
        {
            ServiceInstances.Logger.Info("Starting HTTP service.");

            _httpListener = new HttpListener();

            //string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //path = System.IO.Path.GetDirectoryName(path) + System.IO.Path.DirectorySeparatorChar;

            _allowExtendedLogging = ServiceInstances.Configuration.GetValue("DebugAllowExtendedLogging", false);

            string uriText = ServiceInstances.Configuration.GetValue("HttpListenerUri", string.Empty);
            if (string.IsNullOrEmpty(uriText))
            {
                ServiceInstances.Logger.Warn("HTTP service listening point is not set. HTTP service is not started.");
                return;
            }

            ServiceInstances.Logger.Info("HTTP Service is starting:");

            // TODO:
            // - add ThreadPool for request serving
            // - make sure server listens to remote connections

            //_httpHost = System.Web.Hosting.ApplicationHost.CreateApplicationHost(typeof(VreHttpHost),
            //    uri.LocalPath, path) as VreHttpHost;
            _rsp = new RemoteServiceProvider();

            // BEGIN: Multiple URI support

            Uri uri = new Uri(uriText.Replace("+", "localhost").Replace("*", "localhost"));

            _path = uri.LocalPath;

            _httpListener.Prefixes.Add(uriText);
            _listeners.Add(uriText);
            ServiceInstances.Logger.Info("- " + uriText);

            // END: Multiple URI support

            _httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            _httpListener.Start();
            _httpListener.BeginGetContext(httpCallback, null);

            ServiceInstances.Logger.Info("HTTP Service started.");

            if (_allowExtendedLogging)
                ServiceInstances.RequestLogger.Info("HTTP Service started");
        }

        public static void PerformShutdown()
        {
            if (null != _httpListener)
            {
                ServiceInstances.Logger.Info("HTTP Service is stopping.");
                _httpListener.Stop();
                _httpListener.Close();
                _httpListener = null;
                ServiceInstances.Logger.Info("HTTP Service stopped.");
            }
        }

        //http://msdn.microsoft.com/en-us/magazine/cc163879.aspx

        private static void httpCallback(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                if (null == _httpListener) return;  // shutdown case
                if (!_httpListener.IsListening) return;  // shutdown case

                HttpListenerContext ctx = _httpListener.EndGetContext(ar);

                _httpListener.BeginGetContext(httpCallback, null);

                try
                {
                    // TODO: configurable required SSL check
                    // Currently done in Vre.Server.RemoveService.DataService as all other services may be non-secure
                    // ctx.Request.IsSecureConnection

                    string browserKey = Statistics.GetBrowserId(ctx);

                    HttpServiceRequest rq = new HttpServiceRequest(ctx, _path);

                    if (_allowExtendedLogging)
                    {
                        ClientSession cs = rq.UserInfo.Session;
                        if (cs != null)
                            ServiceInstances.RequestLogger.Info("Session={0}; BK={1}; URL={2}", cs, browserKey, ctx.Request.Url);
                        else
                            ServiceInstances.RequestLogger.Info("Anonymous; BK={0}; URL={1}", browserKey, ctx.Request.Url);
                    }

                    if (!rq.UserInfo.StaleSession)
                    {
                        try
                        {
                            if (rq.UserInfo.Session != null) rq.UserInfo.Session.Resume();
                            _rsp.ProcessRequest(rq);
                            rq.UpdateResponse(ctx.Response);
                        }
                        catch (Exception ex)
                        {
                            rq.UpdateResponse(ctx.Response, ex);
                        }
                        finally
                        {
                            if (rq.UserInfo.Session != null) rq.UserInfo.Session.Disconnect();
                        }
                    }

                    if (rq.Response.HoldResponseForServerPush)
                        Experimental.HttpServerPush.StartServerPushThread(ctx);
                    else 
                        ctx.Response.Close();
                }
                catch (HttpListenerException ex)  // these seem to be useless; just flooding logs
                {
                    ServiceInstances.Logger.Error("HTTP request processing failed: {0}", ex.Message);
                }
                catch (Exception ex)
                {
                    //ctx.Response.Abort();
                    ctx.Response.StatusCode = 500;
                    ctx.Response.StatusDescription = "Server error.";
                    ctx.Response.Close();
                    ServiceInstances.Logger.Error("HTTP request processing failed: {0}", ex);
                }
            }
        }
    }
}