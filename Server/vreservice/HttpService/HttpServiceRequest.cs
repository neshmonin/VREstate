using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.RemoteService;

namespace Vre.Server.HttpService
{
    internal class HttpServiceRequest : IServiceRequest
    {
        public static Dictionary<string, string> ContentTypeByExtension = new Dictionary<string, string>();
        private static bool _allowExtendedLogging = false;
        private static int _fileBufferSize = 16384;

        class RemoteUserInfo : IRemoteUserInfo
        {
            public RemoteUserInfo(IPEndPoint ep, NameValueCollection headers, ServiceQuery query)
            {
                string sidFromHttpHeader = headers["sid"];
                if (string.IsNullOrWhiteSpace(sidFromHttpHeader)) sidFromHttpHeader = query["sid"];
                if (null != sidFromHttpHeader)
                {
                    Session = ServiceInstances.SessionStore[sidFromHttpHeader];
                    if (null == Session) StaleSession = true;
                }
                else
                {
                    Session = null;
                    StaleSession = false;
                }

                EndPoint = ep;
            }
            public IPEndPoint EndPoint { get; private set; }
            public ClientSession Session { get; private set; }
            public bool StaleSession { get; private set; }
        }

        class RequestData : IRequestData
        {
            private static string _serverRootPath = null;
            private Uri _rawRequest;
            public RequestData(HttpListenerRequest request, string path, ServiceQuery query)
            {
                string type = request.HttpMethod;

                // http://en.wikipedia.org/wiki/Representational_State_Transfer#RESTful_web_services
                if (type.Equals("GET")) Type = RequestType.Get;
                else if (type.Equals("PUT")) Type = RequestType.Update;
                else if (type.Equals("POST")) Type = RequestType.Insert;
                else if (type.Equals("DELETE")) Type = RequestType.Delete;
                else throw new ArgumentException("Unknown HTTP method.");

                _rawRequest = request.Url;
                Path = path;
                Query = query;

                // either HTTPS (SSL) or local connection is required to qualify as secure
                IsSecureConnection = request.IsSecureConnection | IsCallerSecure(request.RemoteEndPoint.Address);

                if (request.ContentLength64 > 0) Data = JavaScriptHelper.JsonToClientData(request.InputStream);
                else Data = null;
            }
            public RequestType Type { get; private set; }
            public string Path { get; private set; }
            public ServiceQuery Query { get; private set; }
            public ClientData Data { get; private set; }
            public bool IsSecureConnection { get; private set; }
            public string ConstructClientRootUri()
            {
                if (null == _serverRootPath)
                {
                    string uriText = ServiceInstances.Configuration.GetValue("HttpListenerUri", string.Empty);
                    Uri uri = new Uri(uriText.Replace("+", "localhost").Replace("*", "localhost"));
                    _serverRootPath = uri.LocalPath;
                }

                return string.Format("{0}://{1}{2}",
                    _rawRequest.Scheme, _rawRequest.Authority, _serverRootPath);
            }

            public static bool IsCallerSecure(IPAddress address)
            {
                bool result = false;
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    result = address.Equals(IPAddress.Loopback);
                }
                else if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    result = address.Equals(IPAddress.IPv6Loopback);
                }
                return result;
            }
        }

        class ResponseData : IResponseData
        {
            public ResponseData(Stream httpResponseStream)
            {
                ResponseCode = HttpStatusCode.Unused;
                ResponseCodeDescription = string.Empty;
                Data = null;
                DataStream = httpResponseStream;
                DataStreamContentType = null;
                DataPhysicalLocation = null;
            }
            public HttpStatusCode ResponseCode { get; set; }
            public string ResponseCodeDescription { get; set; }
            public ClientData Data { get; set; }
            public string DataStreamContentType { get; set; }
            public Stream DataStream { get; private set; }
            public string DataPhysicalLocation { get; set; }
            public bool HoldResponseForServerPush { get; set; }
        }

        private static void initialize()
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

            ContentTypeByExtension.Add("js", "application/javascript");
            ContentTypeByExtension.Add("json", "application/json");
            ContentTypeByExtension.Add("kml", "application/vnd.google-earth.kml+xml");
            ContentTypeByExtension.Add("kmz", "application/vnd.google-earth.kmz");

            _allowExtendedLogging = ServiceInstances.Configuration.GetValue("DebugAllowExtendedLogging", false);

            _fileBufferSize = ServiceInstances.Configuration.GetValue("FileStreamingBufferSize", 16384);
        }

        public IRemoteUserInfo UserInfo { get; private set; }
        public IRequestData Request { get; private set; }
        public IResponseData Response { get; private set; }

        static HttpServiceRequest()
        {
            lock (ContentTypeByExtension)
            {
                initialize();
            }
        }

        public HttpServiceRequest(HttpListenerContext ctx, string servicePath)
        {
            // TODO: verify this is valid:
            // ctx.Request.ContentEncoding

            // TODO: verify this is acceptable:
            // ctx.Request.ContentType

            ServiceQuery query = new ServiceQuery(ctx.Request.QueryString);

            string file = ctx.Request.Url.LocalPath;//.Replace("/", "");
            if (file.StartsWith(servicePath)) file = file.Remove(0, servicePath.Length);

            UserInfo = new RemoteUserInfo(ctx.Request.RemoteEndPoint, ctx.Request.Headers, query);
            Request = new RequestData(ctx.Request, file, query);
            Response = new ResponseData(new MemoryStream());

            // update trusted value for this request: managers do not get request object!
            if (UserInfo.Session != null)
                UserInfo.Session.TrustedConnection = Request.IsSecureConnection;

            if (UserInfo.StaleSession)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                ctx.Response.StatusDescription = "Session ID is invalid or dropped by timeout.";
                if (_allowExtendedLogging)
                    ServiceInstances.Logger.Error(string.Format(
                        "HTTP request referred to unknows session ID from {0}.",
                        ctx.Request.RemoteEndPoint));
                else
                    ServiceInstances.Logger.Error("HTTP request referred to unknows session ID.");
            }
        }

        public void UpdateResponse(HttpListenerResponse response)
        {
            if (Response.DataStreamContentType != null)
            {
                string type = Response.DataStreamContentType;
                if (!type.Contains("/"))  // result if a file extension rather a MIME type: convert it
                {
                    if (!ContentTypeByExtension.TryGetValue(type, out type))
                        throw new InvalidDataException("Response type in not known.");
                }
                response.ContentType = type;
            }

            response.StatusCode = (int)Response.ResponseCode;
            response.StatusDescription = Response.ResponseCodeDescription;

            if (Response.Data != null)
            {
                //using (StreamWriter sw = new StreamWriter(response.OutputStream))
                //    sw.Write(JavaScriptHelper.ClientDataToJson(Response.Data));

                response.ContentEncoding = Encoding.UTF8;
                response.ContentType = ContentTypeByExtension["json"];

                byte[] resp = Encoding.UTF8.GetBytes(JavaScriptHelper.ClientDataToJson(Response.Data));
                response.OutputStream.Write(resp, 0, resp.Length);
            }
            else if (Response.DataStream.Length > 0)
            {
                if (Response.DataStream.CanSeek) Response.DataStream.Seek(0, SeekOrigin.Begin);  // safeguard
                Response.DataStream.CopyTo(response.OutputStream);
            }
            else if (Response.DataPhysicalLocation != null)
            {
                // stream file to response
                byte[] buffer = new byte[_fileBufferSize];
                using (Stream fs = File.Open(Response.DataPhysicalLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    int read;
                    do
                    {
                        read = fs.Read(buffer, 0, _fileBufferSize);
                        response.OutputStream.Write(buffer, 0, read);
                    } while (read > 0);
                }
            }
        }

        public void UpdateResponse(HttpListenerResponse response, Exception e)
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
    }
}