﻿using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using Vre.Server.BusinessLogic;
using Vre.Server.RemoteService;

namespace Vre.Server.HttpService
{
    internal class HttpServiceRequest : IServiceRequest
    {
        public delegate void ProcessResponse(object respImpl, IResponseData resp, Exception ex);
                
        class RemoteUserInfo : IRemoteUserInfo
        {
            public RemoteUserInfo(string browserKey, NameValueCollection headers, ServiceQuery query)
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

				BrowserKey = browserKey;
            }
			public string BrowserKey { get; private set; }
            public ClientSession Session { get; private set; }
            public bool StaleSession { get; private set; }
        }

        class RequestData : IRequestData
        {
            private static string _serverRootPath = null;
            private Uri _rawRequest;
			public RequestData(HttpListenerRequest request, IPEndPoint ep, Uri referer, string path, ServiceQuery query, long bodySizeLimit)
            {
                string type = request.HttpMethod;

                // http://en.wikipedia.org/wiki/Representational_State_Transfer#RESTful_web_services
                if (type.Equals("GET")) Type = RequestType.Get;
                else if (type.Equals("PUT")) Type = RequestType.Update;
                else if (type.Equals("POST")) Type = RequestType.Insert;
                else if (type.Equals("DELETE")) Type = RequestType.Delete;
                else throw new ArgumentException("Unknown HTTP method.");

                _rawRequest = request.Url;
				Referer = referer;
                Path = path;
				PathSegments = path.Split('/');
                Query = query;
				EndPoint = ep;
				FormData = null;

                // either HTTPS (SSL) or local connection is required to qualify as secure
                IsSecureConnection = request.IsSecureConnection | IsCallerSecure(request.RemoteEndPoint.Address);

                Data = null;
                RawData = null;
                if ((request.ContentLength64 > 0) && (request.ContentLength64 <= bodySizeLimit))
                {
                    if (request.ContentType.Contains("application/json"))
                    {
						try
						{
							Data = JavaScriptHelper.JsonToClientData(request.InputStream);
						}
						catch
						{
							Data = null;
						}
                    }
					else if (request.ContentType.Contains("multipart/form-data"))
					{
						FormData = MultipartFormData.Parse(request);
					}
                    if ((null == Data) && (null == FormData))
                    {
						RawDataContentType = request.ContentType;
                        RawData = new byte[request.ContentLength64];
                        request.InputStream.Read(RawData, 0, RawData.Length);
                    }
                }
            }

			public Uri Referer { get; private set; }
			public RequestType Type { get; private set; }
            public string Path { get; private set; }
			public string[] PathSegments { get; private set; }
            public ServiceQuery Query { get; private set; }
            public ClientData Data { get; private set; }
			public string RawDataContentType { get; private set; }
            public byte[] RawData { get; private set; }
			public IPEndPoint EndPoint { get; private set; }
			public bool IsSecureConnection { get; private set; }
			public IMultipartFormData FormData { get; private set; }
            public string ConstructClientRootUri()
            {
                if (null == _serverRootPath)
                {
                    string uriText = Configuration.HttpService.ListenerUri.Value;
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

        public class ResponseData : IResponseData
        {
            private ProcessResponse _responseProc;
            private object _respImpl;
            public ResponseData(Stream httpResponseStream, object respImpl, ProcessResponse proc)
            {
                ResponseCode = HttpStatusCode.Unused;
                ResponseCodeDescription = string.Empty;
                Data = null;
                DataStream = httpResponseStream;
                DataStreamContentType = null;
                DataPhysicalLocation = null;
                RedirectionUrl = null;
				HoldResponseForServerPush = false;
				ClientCaching = true;
                _respImpl = respImpl;
                _responseProc = proc;
            }
            public HttpStatusCode ResponseCode { get; set; }
            public string ResponseCodeDescription { get; set; }
            public ClientData Data { get; set; }
            public string DataStreamContentType { get; set; }
            public Stream DataStream { get; private set; }
            public string DataPhysicalLocation { get; set; }
            public bool HoldResponseForServerPush { get; set; }
			public bool ClientCaching { get; set; }
            public string RedirectionUrl { get; set; }
            public void ProcessResponse() { _responseProc(_respImpl, this, null); }
            public void ProcessResponse(Exception ex) { _responseProc(_respImpl, this, ex); }
        }

        public IRemoteUserInfo UserInfo { get; private set; }
        public IRequestData Request { get; private set; }
        public IResponseData Response { get; private set; }

        public HttpServiceRequest(HttpListenerContext ctx, string browserKey, Uri referer,
			string servicePath, long requestBodySizeLimit, ProcessResponse proc)
        {
            // TODO: verify this is valid:
            // ctx.Request.ContentEncoding

            // TODO: verify this is acceptable:
            // ctx.Request.ContentType

            ServiceQuery query = new ServiceQuery(ctx.Request.Headers,
				HttpUtility.ParseQueryString(HttpUtility.UrlDecode(ctx.Request.Url.Query)));

            string file = ctx.Request.Url.LocalPath;//.Replace("/", "");
            if (file.StartsWith(servicePath)) file = file.Remove(0, servicePath.Length);

            UserInfo = new RemoteUserInfo(browserKey, ctx.Request.Headers, query);
			Request = new RequestData(ctx.Request, ctx.Request.RemoteEndPoint, referer, file, query, requestBodySizeLimit);
            Response = new ResponseData(new MemoryStream(), ctx.Response, proc);

            // update trusted value for this request: managers do not get request object!
            if (UserInfo.Session != null)
                UserInfo.Session.TrustedConnection = Request.IsSecureConnection;

            if (UserInfo.StaleSession)
            {
                Response.ResponseCode = HttpStatusCode.RequestTimeout;
                Response.ResponseCodeDescription = "Session ID is invalid or dropped by timeout.";

                ServiceInstances.Logger.Error(string.Format(
                    "HTTP request referred to unknown session ID from {0}.",
                    ctx.Request.RemoteEndPoint));
                //else
                //    ServiceInstances.Logger.Error("HTTP request referred to unknown session ID.");
            }
        }
    }
}