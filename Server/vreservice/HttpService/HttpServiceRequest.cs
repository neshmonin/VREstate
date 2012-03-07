using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Vre.Server.BusinessLogic;
using Vre.Server.RemoteService;

namespace Vre.Server.HttpService
{
    internal class HttpServiceRequest : IServiceRequest
    {
        private static Dictionary<string, string> _contentTypeByExtension = new Dictionary<string, string>();

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
            public RequestData(Uri rawRequest, string type, string path, ServiceQuery query, Stream data, bool isSecureConnection)
            {
                // http://en.wikipedia.org/wiki/Representational_State_Transfer#RESTful_web_services
                if (type.Equals("GET")) Type = RequestType.Get;
                else if (type.Equals("PUT")) Type = RequestType.Update;
                else if (type.Equals("POST")) Type = RequestType.Insert;
                else if (type.Equals("DELETE")) Type = RequestType.Delete;
                else throw new ArgumentException("Unknown HTTP method.");

                _rawRequest = rawRequest;
                Path = path;
                Query = query;
                IsSecureConnection = isSecureConnection;

                if (data != null) Data = JavaScriptHelper.JsonToClientData(data);
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
            }
            public HttpStatusCode ResponseCode { get; set; }
            public string ResponseCodeDescription { get; set; }
            public ClientData Data { get; set; }
            public string DataStreamContentType { get; set; }
            public Stream DataStream { get; private set; }
            public bool HoldResponseForServerPush { get; set; }
        }

        private static void initialize()
        {
            // http://www.iana.org/assignments/media-types/

            _contentTypeByExtension.Add("html", "text/html");
            _contentTypeByExtension.Add("htm", "text/html");
            _contentTypeByExtension.Add("txt", "text/plain");
            _contentTypeByExtension.Add("xml", "text/xml");
            _contentTypeByExtension.Add("css", "text/css");

            _contentTypeByExtension.Add("gif", "image/gif");
            _contentTypeByExtension.Add("jpeg", "image/jpeg");
            _contentTypeByExtension.Add("jpg", "image/jpeg");
            _contentTypeByExtension.Add("png", "image/png");

            _contentTypeByExtension.Add("js", "application/javascript");
            _contentTypeByExtension.Add("json", "application/json");
            _contentTypeByExtension.Add("kml", "application/vnd.google-earth.kml+xml");
            _contentTypeByExtension.Add("kmz", "application/vnd.google-earth.kmz");
        }

        public IRemoteUserInfo UserInfo { get; private set; }
        public IRequestData Request { get; private set; }
        public IResponseData Response { get; private set; }

        public HttpServiceRequest(HttpListenerContext ctx, string servicePath)
        {
            lock (_contentTypeByExtension)
            {
                if (_contentTypeByExtension.Count < 1) initialize();
            }

            // TODO: verify this is valid:
            // ctx.Request.ContentEncoding

            // TODO: verify this is acceptable:
            // ctx.Request.ContentType

            ServiceQuery query = new ServiceQuery(ctx.Request.QueryString);

            string file = ctx.Request.Url.LocalPath;//.Replace("/", "");
            if (file.StartsWith(servicePath)) file = file.Remove(0, servicePath.Length);

            UserInfo = new RemoteUserInfo(ctx.Request.RemoteEndPoint, ctx.Request.Headers, query);
            Request = new RequestData(ctx.Request.Url, ctx.Request.HttpMethod, file, query,
                (ctx.Request.ContentLength64 > 0) ? ctx.Request.InputStream : null,
                ctx.Request.IsSecureConnection);
            Response = new ResponseData(ctx.Response.OutputStream);

            if (UserInfo.StaleSession)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                ctx.Response.StatusDescription = "Session ID is invalid or dropped by timeout.";
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
                    if (!_contentTypeByExtension.TryGetValue(type, out type))
                        throw new InvalidDataException("Response type in not known.");
                }
                response.ContentType = type;
            }
            else if (Response.Data != null)
            {
                //using (StreamWriter sw = new StreamWriter(response.OutputStream))
                //    sw.Write(JavaScriptHelper.ClientDataToJson(Response.Data));

                response.ContentEncoding = Encoding.UTF8;
                response.ContentType = _contentTypeByExtension["json"];

                byte[] resp = Encoding.UTF8.GetBytes(JavaScriptHelper.ClientDataToJson(Response.Data));
                response.OutputStream.Write(resp, 0, resp.Length);
            }

            response.StatusCode = (int)Response.ResponseCode;
            response.StatusDescription = Response.ResponseCodeDescription;
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
                response.StatusDescription = "Current user has no permission to view this object.";
                ServiceInstances.Logger.Error("Attempt to retrieve object not granted: {0}", e.Message);
            }
            else if (e is ArgumentException)
            {
                response.StatusCode = (int)HttpStatusCode.NotImplemented;
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
                response.StatusCode = (int)HttpStatusCode.Conflict;
                response.StatusDescription = "The data passed to server is not valid.";
                ServiceInstances.Logger.Error("Request processing fauled: {0}", e.Message);
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