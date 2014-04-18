using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.RemoteService;

namespace Vre.Server.HttpService
{
    /// <summary>
    /// Note that current design allows only one such service per instance (see hard-coded configuration parameter names).
    /// </summary>
    internal abstract class HttpServiceBase
    {
        public static Dictionary<string, string> ContentTypeByExtension = new Dictionary<string, string>();

        private static DateTime _buildTime = DateTime.MinValue;
        private static char[] _invalidPathChars = Path.GetInvalidPathChars();
        private static char[] _invalidFileNameChars = Path.GetInvalidFileNameChars();

        private List<string> _listeners = new List<string>();

        public string Status { get; private set; }
        public string[] Listeners { get { lock (_listeners) return _listeners.ToArray(); } }

        private HttpListener _httpListener;
        private List<string> _listeningHostAliasList;
        private List<string> _referringHostList;

        protected string _path;

        protected string _name;

        protected HashSet<string> _allowedFileExtensions = new HashSet<string>();

		private DateTime _createTime;
		
		public HttpServiceBase(string serviceName)
        {
            _name = serviceName;
            Status = "Stopped";

	        _createTime = DateTime.UtcNow;

            if (0 == ContentTypeByExtension.Count) initializeContentType();

			// TODO: Static configuration
			_allowedFileExtensions = new HashSet<string>(CsvUtilities.Split(Configuration.HttpService.FileService.AllowedExtensions.Value));

            _listeningHostAliasList = new List<string>(CsvUtilities.Split(Configuration.HttpService.ListeningHostAliasList.Value));

            _referringHostList = new List<string>(CsvUtilities.Split(Configuration.HttpService.ReferringHostAliasList.Value));
        }

        public void PerformStartup()
        {
            ServiceInstances.Logger.Info("Starting {0}...", _name);
            Status = "Starting";

			new Thread(queryStatThread) { Name = "HTTPQueryStat", IsBackground = true, Priority = ThreadPriority.Lowest }.Start();

            _httpListener = new HttpListener();

			string uriText = Configuration.HttpService.ListenerUri.Value;
            if (string.IsNullOrEmpty(uriText))
            {
                ServiceInstances.Logger.Warn("HTTP service listening point is not set. HTTP service is not started.");
                return;
            }

            ServiceInstances.Logger.Info("{0} is starting:", _name);

            // TODO:
            // - add ThreadPool for request serving
            // - make sure server listens to remote connections

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

            ServiceInstances.Logger.Info("{0} started.", _name);
            ServiceInstances.Logger.Info("Allowed hosts list: {0}", CsvUtilities.ToString(_listeningHostAliasList));
            ServiceInstances.Logger.Info("Referring hosts list: {0}", CsvUtilities.ToString(_referringHostList));

            if (Configuration.Debug.AllowExtendedLogging.Value)
                ServiceInstances.RequestLogger.Info("{0} started", _name);

            Status = "Running";
        }

        public void PerformShutdown()
        {
            if (null != _httpListener)
            {
                Status = "Stopping";
                ServiceInstances.Logger.Info("{0} is stopping...", _name);
                _httpListener.Stop();
                _httpListener.Close();
                _httpListener = null;
                ServiceInstances.Logger.Info("{0} stopped.", _name);
            }

	        _queryStatExit.Set();

            Status = "Stopped";
        }

        //http://msdn.microsoft.com/en-us/magazine/cc163879.aspx

        private void httpCallback(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                if (null == _httpListener) return;  // shutdown case
                if (!_httpListener.IsListening) return;  // shutdown case

                if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
                    Thread.CurrentThread.Name = "HTTPproc#" + Thread.CurrentThread.ManagedThreadId.ToString();

                HttpListenerContext ctx = null;

                try { ctx = _httpListener.EndGetContext(ar); }
                catch { }  // make sure this does not prevent server from accepting further requests

                _httpListener.BeginGetContext(httpCallback, null);

                if (null == ctx) return;  // error case; just give up the request
                long st = System.Diagnostics.Stopwatch.GetTimestamp();
                try
                {
                    string hostName = ctx.Request.Headers["Host"];

                    if (hostName != null)
                    {
                        if (!_listeningHostAliasList.Contains(hostName))
                        {
                            ServiceInstances.Logger.Warn("HTTP Host: {0}", hostName);
                            ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            ctx.Response.Close();
                            return;
                        }
                    }

                    string origin = ctx.Request.Headers["Origin"];
                    if (origin != null)  // CORS support (http://en.wikipedia.org/wiki/Cross-Origin_Resource_Sharing)
                    {
                        if (_referringHostList.Contains(origin))
                        {
                            ctx.Response.Headers.Add("Access-Control-Allow-Origin", origin);

                            string acrMethod = ctx.Request.Headers["Access-Control-Request-Method"];
                            string acrHeaders = ctx.Request.Headers["Access-Control-Request-Headers"];

                            if (acrMethod != null)
                            {
                                ServiceInstances.Logger.Debug("CORS Meth: {0}", acrMethod);
                                ctx.Response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,PUT,DELETE");
                                if (acrHeaders != null)
                                    ctx.Response.Headers.Add("Access-Control-Allow-Headers", acrHeaders);
                            }

                            ctx.Response.Headers.Add("Access-Control-Max-Age", "3600");  // TODO: ???
                        }
                        else
                        {
                            ServiceInstances.Logger.Warn("CORS Origin: {0}", origin);
                            ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            ctx.Response.Close();
                            return;
                        }
                    }

                    string browserKey = Statistics.GetBrowserId(ctx);

                    IResponseData r = process(browserKey, ctx, processResponse);
                    if (!r.HoldResponseForServerPush)
                    {
                        processResponse(ctx.Response, r, null);
                        ctx.Response.Close();
                    }
                }
                catch (HttpListenerException ex)  // these stack traces seem to be useless; just flooding logs
                {
                    ServiceInstances.Logger.Error("HTTP request processing failed: {0}", ex.Message);
                }
                catch (Exception ex)
                {
                    try
                    {
                        processResponse(ctx.Response, null, ex);
                        ctx.Response.Close();
                    }
                    catch { }  // make sure this does not break server with unhandled exception
                }

				// calculate request processing time
				//
                long el = ((System.Diagnostics.Stopwatch.GetTimestamp() - st) * 1000) / System.Diagnostics.Stopwatch.Frequency;
				enqueueQueryStat(el);
				if (el >= 1000) ServiceInstances.Logger.Warn("Request processed in {0} ms !!!@@@", el);
				else if (Configuration.Debug.AllowExtendedLogging.Value) ServiceInstances.Logger.Debug("Request processed in {0} ms", el);
            }
        }

        protected abstract IResponseData process(string browserKey, HttpListenerContext ctx, HttpServiceRequest.ProcessResponse proc);

        private void processResponse(object respImpl, IResponseData resp, Exception ex)
        {
            HttpListenerResponse ctx = respImpl as HttpListenerResponse;
            try
            {
                if (ctx != null)
                {
                    if (ex != null) updateResponse(ctx, ex);
                    else if (resp != null) updateResponse(ctx, resp);
                }
            }
            finally
            {
                if (ctx != null) try { ctx.Close(); } catch {}
            }
        }

        private void updateResponse(HttpListenerResponse response, IResponseData data)
        {
			if (!data.ClientCaching) disableBrowserCaching(response);

            if (data.RedirectionUrl != null)
            {
                response.Redirect(data.RedirectionUrl);
            }
            else
            {
                if (data.DataStreamContentType != null)
                {
                    string type = data.DataStreamContentType;
                    if (!type.Contains("/"))  // result if a file extension rather a MIME type: convert it
                    {
                        if (!ContentTypeByExtension.TryGetValue(type, out type))
                            throw new InvalidDataException("Response type in not known.");
                    }
                    response.ContentType = type;
                }

                response.StatusCode = (int)data.ResponseCode;
                response.StatusDescription = data.ResponseCodeDescription;

                if (data.Data != null)
                {
                    //using (StreamWriter sw = new StreamWriter(response.OutputStream))
                    //    sw.Write(JavaScriptHelper.ClientDataToJson(Response.Data));

                    response.ContentEncoding = Encoding.UTF8;
                    response.ContentType = ContentTypeByExtension["json"];

                    byte[] resp = Encoding.UTF8.GetBytes(JavaScriptHelper.ClientDataToJson(data.Data));
                    response.OutputStream.Write(resp, 0, resp.Length);
                }
                else if (data.DataStream.Length > 0)
                {
					if (data.DataStream.CanSeek) data.DataStream.Seek(0, SeekOrigin.Begin);  // safeguard
                    data.DataStream.CopyTo(response.OutputStream);
                }
                else if (data.DataPhysicalLocation != null)
                {
                    // stream file to response
                    byte[] buffer = new byte[Configuration.HttpService.FileService.StreamingBufferSize.Value];
                    using (Stream fs = File.Open(data.DataPhysicalLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        int read;
                        do
                        {
                            read = fs.Read(buffer, 0, buffer.Length);
                            response.OutputStream.Write(buffer, 0, read);
                        } while (read > 0);
                    }
                }
            }  // ! redirection
        }

		private static void disableBrowserCaching(HttpListenerResponse response)
		{
			// http://stackoverflow.com/questions/49547/making-sure-a-web-page-is-not-cached-across-all-browsers
			response.Headers[HttpResponseHeader.CacheControl] = "no-cache, no-store, must-revalidate";
			response.Headers[HttpResponseHeader.Pragma] = "no-cache";
			response.Headers[HttpResponseHeader.Expires] = "0";
		}

        private void updateResponse(HttpListenerResponse response, Exception e)
        {
            
            if (e is ObjectExistsException)
            {
                response.StatusCode = (int)HttpStatusCode.Conflict;
                if (Configuration.Debug.AllowExtendedLogging.Value)
                {
                    response.StatusDescription = "Entity already exists: " + e.Message;
                    ServiceInstances.Logger.Error("Entity already exists: {0}", e);
                }
                else
                {
                    response.StatusDescription = "Entity already exists.";
                    ServiceInstances.Logger.Error("Entity already exists: {0}", e.Message);
                }
            }
            else if (e is ExpiredException)
            {
                response.StatusCode = (int)HttpStatusCode.Gone;
				if (Configuration.Debug.AllowExtendedLogging.Value)
                {
                    response.StatusDescription = e.Message != null ? e.Message : "Content expired.";
                    ServiceInstances.Logger.Error("Requested entity is expired: {0}", e);
                }
                else
                {
                    response.StatusDescription = "Content expired.";
                    ServiceInstances.Logger.Error("Requested entity is expired: {0}", e.Message);
                }
            }
            else if (e is FileNotFoundException)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
				if (Configuration.Debug.AllowExtendedLogging.Value)
                {
                    response.StatusDescription = "Content does not exist: " + e.Message;
                    ServiceInstances.Logger.Error("HTTP request for unknown entity: {0}", e);
                }
                else
                {
                    response.StatusDescription = "Content does not exist.";
                    ServiceInstances.Logger.Error("HTTP request for unknown entity: {0}", e.Message);
                }
            }
            else if (e is PermissionException)
            {
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                response.StatusDescription = e.Message;// "Current user has no permission to view this object.";
				if (Configuration.Debug.AllowExtendedLogging.Value)
                {
                    ServiceInstances.Logger.Error("Attempt to retrieve object not granted: {0}", e);
                }
                else
                {
                    ServiceInstances.Logger.Error("Attempt to retrieve object not granted: {0}", e.Message);
                }
            }
            else if (e is InvalidOperationException)
            {
                response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                response.StatusDescription = e.Message;
				if (Configuration.Debug.AllowExtendedLogging.Value)
                {
                    ServiceInstances.Logger.Error("Cannot perform operation: {0}", e);
                }
                else
                {
                    ServiceInstances.Logger.Error("Cannot perform operation: {0}", e.Message);
                }
            }
            else if (e is StaleObjectStateException)
            {
                response.StatusCode = (int)HttpStatusCode.Conflict;
                response.StatusDescription = e.Message;
				if (Configuration.Debug.AllowExtendedLogging.Value)
                {
                    ServiceInstances.Logger.Error("Stale object: {0}", e);
                }
                else
                {
                    ServiceInstances.Logger.Error("Stale object: {0}", e.Message);
                }
            }
            else if (e is ArgumentException)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
				if (Configuration.Debug.AllowExtendedLogging.Value)
                {
                    response.StatusDescription = "Argument error: " + e.Message;
                    ServiceInstances.Logger.Error("{0}", e);
                }
                else
                {
                    response.StatusDescription = "Argument error.";
                    ServiceInstances.Logger.Error("{0}", e.Message);
                }
            }
            else if (e is NotImplementedException)
            {
                response.StatusCode = (int)HttpStatusCode.NotImplemented;
                response.StatusDescription = "Service not implemented.";
				if (Configuration.Debug.AllowExtendedLogging.Value)
                {
                    ServiceInstances.Logger.Error("Service not implemented: {0}", e);
                }
                else
                {
                    ServiceInstances.Logger.Error("Service not implemented: {0}", e.Message);
                }
            }
            else if (e is InvalidDataException)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
				if (Configuration.Debug.AllowExtendedLogging.Value)
                {
                    response.StatusDescription = "The data passed to server is not valid: " + e.Message;
                    ServiceInstances.Logger.Error("Request processing failed: {0}", e);
                }
                else
                {
                    response.StatusDescription = "The data passed to server is not valid.";
                    ServiceInstances.Logger.Error("Request processing failed: {0}", e.Message);
                }
            }
            else if (e is HttpListenerException)
            {
				if (Configuration.Debug.AllowExtendedLogging.Value)
                {
                    ServiceInstances.Logger.Error("HTTP request processing failed: {0}", e);
                }
                else
                {
                    ServiceInstances.Logger.Error("HTTP request processing failed: {0}", e.Message);
                }
                // no need to set status here as connection is no longer workable
            }
            else
            {
                //ctx.Response.Abort();
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.StatusDescription = "Server error.";
                if (e != null)
                    ServiceInstances.Logger.Error("HTTP request processing failed: {0}", e);
                else
                    ServiceInstances.Logger.Error("HTTP request processing failed.");
            }
        }

        public void ProcessFileRequest(string file, IResponseData response)
        {
            if (file.Equals("version")) rq_version(response);
            else if (file.Equals("humans.txt")) rq_text(1, response);
            else if (file.Equals("robots.txt")) rq_text(0, response);
            else if (file.Equals("base")) rq_redirect(0, response);
			else if (file.Equals("health")) rq_health(response);
			else
            {
				if (file.Equals("view")) file = Configuration.HttpService.ViewClientPath.Value;

                // validate path
                if (!IsPathValid(file, true)) throw new FileNotFoundException("Path is invalid.");

                // validate file type: only certain file types are accessible (!)
                response.DataStreamContentType = Path.GetExtension(file).ToLower().Substring(1);
                if (!_allowedFileExtensions.Contains(response.DataStreamContentType)) throw new FileNotFoundException("File type not known.");

                // getfullpath shall resolve any back-steps (..)
                response.DataPhysicalLocation = Path.GetFullPath(Path.Combine(Configuration.HttpService.FileService.RootPath.Value, file));
                // do not allow going into up-level directories
				if (!response.DataPhysicalLocation.StartsWith(Configuration.HttpService.FileService.RootPath.Value)) throw new FileNotFoundException("File not found.");
                // verify file presence
                if (!File.Exists(response.DataPhysicalLocation)) throw new FileNotFoundException("File not found.");

                response.ResponseCode = HttpStatusCode.OK;
            }
        }

        private static void initializeContentType()
        {
            string mimeFile = Path.Combine(Configuration.ConfigurationFilesPath, "mimetypes.csv");

            try
            {
                if (File.Exists(mimeFile))
                {
                    using (var r = new StreamReader(mimeFile))
                    {
                        while (!r.EndOfStream)
                        {
                            string[] parts = CsvUtilities.Split(r.ReadLine());
                            if (2 == parts.Length)
                                ContentTypeByExtension.Add(parts[0], parts[1]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ServiceInstances.Logger.Error("Error reading MIME types file; using defaults. {0}", e);

                ContentTypeByExtension.Clear();
                // http://www.iana.org/assignments/media-types/

                ContentTypeByExtension.Add("html", "text/html");
                ContentTypeByExtension.Add("htm", "text/html");
                ContentTypeByExtension.Add("txt", "text/plain");
                ContentTypeByExtension.Add("xml", "text/xml");
                ContentTypeByExtension.Add("css", "text/css");

                ContentTypeByExtension.Add("gif", "image/gif");
                ContentTypeByExtension.Add("jpeg", "image/jpeg");
                ContentTypeByExtension.Add("jpg", "image/jpeg");
                ContentTypeByExtension.Add("png", "image/png");
                ContentTypeByExtension.Add("ico", "image/vnd.microsoft.icon");

                ContentTypeByExtension.Add("js", "application/javascript");
                ContentTypeByExtension.Add("json", "application/json");
                ContentTypeByExtension.Add("kml", "application/vnd.google-earth.kml+xml");
                ContentTypeByExtension.Add("kmz", "application/vnd.google-earth.kmz");

                ContentTypeByExtension.Add("pdf", "application/pdf");
            }
        }

        public static bool IsPathValid(string path, bool allowSubfolder)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            if (path.IndexOfAny(_invalidPathChars) >= 0) return false;
            if (!allowSubfolder)
            {
                if (path.IndexOfAny(_invalidFileNameChars) >= 0) return false;
            }

            if (path.Contains("..") || path.StartsWith("\\") || path.StartsWith("/")) return false;

            return true;
        }

        public static DateTime ParseDateTimeParam(string paramValue, DateTime? defaultValue)
        {
            DateTime result;
            if (!DateTime.TryParseExact(paramValue, "yyyy-MM-ddTHH:mm:ssZ",  // TODO: Use 'K' instead of 'Z' to allow time offset specification
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result))
            {
                if (defaultValue.HasValue) result = defaultValue.Value;
                else throw new ArgumentException("The time specified is not valid");
            }
            return result;
        }

        private void rq_version(IResponseData response)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(BuildVersionInformation());
            response.DataStream.Write(buffer, 0, buffer.Length);
            response.DataStreamContentType = "txt";
            response.ResponseCode = HttpStatusCode.OK;
        }

		private void rq_health(IResponseData response)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(Util.HealthInfo.BuildHealthReport());
			response.DataStream.Write(buffer, 0, buffer.Length);
			response.DataStreamContentType = "txt";
			response.ResponseCode = HttpStatusCode.OK;
		}

		private void rq_text(int type, IResponseData response)
        {
            response.ResponseCode = HttpStatusCode.OK;
            response.DataStreamContentType = "txt";

            StringBuilder text = new StringBuilder();

            switch (type)
            {
                case 0:  //robots.txt
                    text.Append("User-agent: *\r\n");
                    text.Append("Disallow: /\r\n");
                    text.Append("Disallow: /harming/humans\r\n");
                    text.Append("Disallow: /ignoring/human/orders\r\n");
                    text.Append("Disallow: /harm/to/self\r\n");
                    break;

                case 1:  // humans.txt
                    if (DateTime.MinValue == _buildTime)
                    {
                        _buildTime = File.GetCreationTime(Assembly.GetExecutingAssembly().Location);
                    }

                    text.Append("/* humanstxt.org */\r\n");
                    text.Append("\r\n");
                    text.Append("/* TEAM */\r\n");
                    text.Append("\tAlexander Neshmonin, CEO and everything, Toronto\r\n");
                    text.Append("\tEugene Simonov, Frontend, Ukraine\r\n");
                    text.Append("\tAndrey Maslyuk, Backend, Toronto\r\n");
                    text.Append("\r\n");
                    text.Append("/* THANKS */\r\n");
                    text.Append("\tVitaly Zholudev\r\n");
                    text.Append("\thttp://last.fm\r\n");
                    text.Append("\thttp://stackoverflow.com\r\n");
                    text.Append("\r\n");
                    text.Append("\r\n");
                    text.Append("/* SITE */\r\n");
                    text.Append("\tLast build: " + _buildTime.ToShortDateString() + "\r\n");
                    text.Append("\tLast update: today\r\n");
                    text.Append("\tStandards: HTTP/1.1\r\n");
                    text.Append("\tLanguages: none\r\n");
                    text.Append("\r\n");
                    text.Append("\r\n");
                    text.Append("/* FRAMEWORKS */\r\n");
                    text.Append("\tMoney: by codekaizen@codeproject");
                    text.Append("\t");
                    break;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(text.ToString());
            response.DataStream.Write(buffer, 0, buffer.Length);
        }

        private void rq_redirect(int type, IResponseData response)
        {
            switch (type)
            {
                case 0:
                    response.RedirectionUrl = "http://3dcondox.com";
                    break;
            }
        }

        public static string BuildVersionInformation()
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("{0}, {1}", VersionGen.ProductName, VersionGen.CopyrightString);

            result.AppendFormat("\r\nVersion: {0}", Assembly.GetExecutingAssembly().GetName().Version);
            result.AppendFormat("\r\nBuild version stamp: {0}", VersionGen.VersionStamp);

#if !DEBUG
            if (VersionGen.IsAlpha)
#endif
            result.AppendFormat(" ALPHA {0:yyyyMMddHHmmss}",
                File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location));

            return result.ToString();
        }

        protected static string prepareCallerInfo(string browserKey, HttpListenerContext ctx, IServiceRequest rq)
        {
            ClientSession cs = (rq != null) ? rq.UserInfo.Session : null;
            string url = Utilities.SanitizeUrl(ctx.Request.Url.ToString());
            if (cs != null)
				return string.Format("Session={0}; BK={1}, REP={2}; {3}; URL={4}; {5}", cs, browserKey, ctx.Request.RemoteEndPoint, ctx.Request.HttpMethod, url, ctx.Request.UrlReferrer);
            else
				return string.Format("Anonymous; BK={0}; REP={1}; {2}; URL={3}; {4}", browserKey, ctx.Request.RemoteEndPoint, ctx.Request.HttpMethod, url, ctx.Request.UrlReferrer);
        }

		#region query statistics
		private ConcurrentQueue<long> _queryStatQueue = new ConcurrentQueue<long>();
		private AutoResetEvent _queryStatTrigger = new AutoResetEvent(false);
		private AutoResetEvent _queryStatExit = new AutoResetEvent(false);

	    private void enqueueQueryStat(long procTime)
	    {
			_queryStatQueue.Enqueue(procTime);
		    _queryStatTrigger.Set();
	    }

	    private void queryStatThread()
	    {
			long minRqTime = long.MaxValue, maxRqTime = 0, rqCnt = 0, lastRqStatCnt = 0;
		    DateTime nextRqStatTime = DateTime.UtcNow.AddHours(1);

			var events = new WaitHandle[2];
		    events[0] = _queryStatTrigger;
		    events[1] = _queryStatExit;

		    while (true)
		    {
			    switch (WaitHandle.WaitAny(events))
			    {
					case 0:
					    {
							long el;
						    while (_queryStatQueue.TryDequeue(out el))
						    {
							    // update statistics
							    //
							    if (minRqTime > el) minRqTime = el;
							    if (maxRqTime < el) maxRqTime = el;
							    rqCnt++;

							    // log statistics
							    //
							    if (((rqCnt - lastRqStatCnt) < 1000) && (DateTime.UtcNow <= nextRqStatTime)) continue;

							    ServiceInstances.Logger.Info(@"Request process stats (uptime is {0:d\ hh\:mm\:ss}):
- total count: {1}
- minimal time (ms): {2}
- maximal time (ms): {3}",
							        DateTime.UtcNow.Subtract(_createTime), rqCnt, minRqTime, maxRqTime);

							    nextRqStatTime = DateTime.UtcNow.AddHours(1);
							    lastRqStatCnt = rqCnt;
						    }
					    }
					    break;

				    default:
					    return;
			    }
		    }
		}
		#endregion
	}
}