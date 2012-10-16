using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.RemoteService;
using System.Reflection;

namespace Vre.Server.HttpService
{
    /// <summary>
    /// Note that current design allows only one such service per instance (see hard-coded configuration parameter names).
    /// </summary>
    internal abstract class HttpServiceBase
    {
        public static Dictionary<string, string> ContentTypeByExtension = new Dictionary<string, string>();

        private static char[] _invalidPathChars = Path.GetInvalidPathChars();
        private static char[] _invalidFileNameChars = Path.GetInvalidFileNameChars();

        private List<string> _listeners = new List<string>();

        public string Status { get; private set; }
        public string[] Listeners { get { lock (_listeners) return _listeners.ToArray(); } }

        private HttpListener _httpListener;

        protected string _path;
        protected int _fileBufferSize = 16384;

        protected bool _allowExtendedLogging;

        protected string _name;

        protected List<string> _allowedFileExtensions = new List<string>();
        protected string _filesRootFolder = null;

        public HttpServiceBase(string serviceName)
        {
            _name = serviceName;
            Status = "Stopped";

            if (0 == ContentTypeByExtension.Count) initializeContentType();
            _fileBufferSize = ServiceInstances.Configuration.GetValue("FileStreamingBufferSize", 16384);
            _allowExtendedLogging = ServiceInstances.Configuration.GetValue("DebugAllowExtendedLogging", false);

            _filesRootFolder = ServiceInstances.Configuration.GetValue("FilesRoot",
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            _allowedFileExtensions = Utilities.FromCsv(ServiceInstances.Configuration.GetValue("AllowedServedFileExtensions", string.Empty));
        }

        public void PerformStartup()
        {
            ServiceInstances.Logger.Info("Starting {0}...", _name);
            Status = "Starting";

            _httpListener = new HttpListener();

            string uriText = ServiceInstances.Configuration.GetValue("HttpListenerUri", string.Empty);
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

            if (_allowExtendedLogging)
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
            Status = "Stopped";
        }

        //http://msdn.microsoft.com/en-us/magazine/cc163879.aspx

        private void httpCallback(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                if (null == _httpListener) return;  // shutdown case
                if (!_httpListener.IsListening) return;  // shutdown case

                HttpListenerContext ctx = null;

                try { ctx = _httpListener.EndGetContext(ar); }
                catch { }  // make sure this does not prevent server from accepting further requests

                _httpListener.BeginGetContext(httpCallback, null);

                if (null == ctx) return;  // error case; just give up the request
                try
                {
                    string browserKey = Statistics.GetBrowserId(ctx);

                    IResponseData r = process(browserKey, ctx);
                    updateResponse(ctx.Response, r);

                    if (r.HoldResponseForServerPush)
                        Experimental.HttpServerPush.StartServerPushThread(ctx);
                    else
                        ctx.Response.Close();
                }
                catch (HttpListenerException ex)  // these stack traces seem to be useless; just flooding logs
                {
                    ServiceInstances.Logger.Error("HTTP request processing failed: {0}", ex.Message);
                }
                catch (Exception ex)
                {
                    try
                    {
                        updateResponse(ctx.Response, ex);
                        ctx.Response.Close();
                    }
                    catch { }  // make sure this does not break server with unhandled exception
                }
            }
        }

        protected abstract IResponseData process(string browserKey, HttpListenerContext ctx);

        private void updateResponse(HttpListenerResponse response, IResponseData data)
        {
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
                    byte[] buffer = new byte[_fileBufferSize];
                    using (Stream fs = File.Open(data.DataPhysicalLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        int read;
                        do
                        {
                            read = fs.Read(buffer, 0, _fileBufferSize);
                            response.OutputStream.Write(buffer, 0, read);
                        } while (read > 0);
                    }
                }
            }  // ! redirection
        }

        private void updateResponse(HttpListenerResponse response, Exception e)
        {
            if (e is FileNotFoundException)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.StatusDescription = "Content does not exist.";
                ServiceInstances.Logger.Error("HTTP request for unknown entity: {0}", e.Message);
            }
            else if (e is PermissionException)
            {
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                response.StatusDescription = e.Message;// "Current user has no permission to view this object.";
                ServiceInstances.Logger.Error("Attempt to retrieve object not granted: {0}", e.Message);
            }
            else if (e is InvalidOperationException)
            {
                response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                response.StatusDescription = e.Message;
                ServiceInstances.Logger.Error("Cannot perform operation: {0}", e.Message);
            }
            else if (e is StaleObjectStateException)
            {
                response.StatusCode = (int)HttpStatusCode.Conflict;
                response.StatusDescription = e.Message;
                ServiceInstances.Logger.Error("Stale object: {0}", e.Message);
            }
            else if (e is ArgumentException)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.StatusDescription = "Argument error.";
                ServiceInstances.Logger.Error("{0}", e.Message);
            }
            else if (e is NotImplementedException)
            {
                response.StatusCode = (int)HttpStatusCode.NotImplemented;
                response.StatusDescription = "Service not implemented.";
                ServiceInstances.Logger.Error("Service not implemented: {0}", e.Message);
            }
            else if (e is InvalidDataException)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.StatusDescription = "The data passed to server is not valid.";
                ServiceInstances.Logger.Error("Request processing fauled: {0}", e.Message);
            }
            else if (e is HttpListenerException)
            {
                ServiceInstances.Logger.Error("HTTP request processing failed: {0}", e.Message);
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
            // validate path
            if (!IsPathValid(file, true)) throw new FileNotFoundException("Path is invalid.");

            // validate file type: only certain file types are accessible (!)
            response.DataStreamContentType = Path.GetExtension(file).ToLower().Substring(1);
            if (!_allowedFileExtensions.Contains(response.DataStreamContentType)) throw new FileNotFoundException("File type not known.");

            // getfullpath shall resolve any back-steps (..)
            response.DataPhysicalLocation = Path.GetFullPath(Path.Combine(_filesRootFolder, file));
            // do not allow going into up-level directories
            if (!response.DataPhysicalLocation.StartsWith(_filesRootFolder)) throw new FileNotFoundException("File not found.");
            // verify file presence
            if (!File.Exists(response.DataPhysicalLocation)) throw new FileNotFoundException("File not found.");

            response.ResponseCode = HttpStatusCode.OK;
        }

        private static void initializeContentType()
        {
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
    }
}