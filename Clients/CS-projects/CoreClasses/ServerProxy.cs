using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Vre.Server;
using Vre.Server.BusinessLogic;
using System.Diagnostics;

namespace CoreClasses
{
    public class ServerResponse
    {
        public ServerResponse(HttpStatusCode code, string codeDescription, ClientData data)
        {
            ResponseCode = code;
            ResponseCodeDescription = codeDescription;
            Data = data;
        }
        public HttpStatusCode ResponseCode { get; private set; }
        public string ResponseCodeDescription { get; private set; }
        public ClientData Data { get; private set; }
    }

    public class ServerProxy : IDisposable
    {
        private static long _generation = 0;

        // Declare delegates to signal the Request stages
        public delegate void RequestBeginsDelegate();
        public delegate void UpdateUiDelegate();
        public delegate void RequestEndDelegate();
        public delegate void RequestFailedDelegate(string description);
        public delegate void SessionTreadExpired();

        public class RequestType
        {
            public string Method { get; private set; }
            private RequestType(string method) { Method = method; }
            public static RequestType Get { get { return new RequestType("GET"); } }
            public static RequestType Insert { get { return new RequestType("POST"); } }
            public static RequestType Update { get { return new RequestType("PUT"); } }
            public static RequestType Delete { get { return new RequestType("DELETE"); } }
        }

        private UpdateUiDelegate _UiUpdater = null;
        private RequestBeginsDelegate _onRequestBegin = null;
        private RequestEndDelegate _onRequestEnd = null;
        private RequestFailedDelegate _onRequestFailed = null;
        private SessionTreadExpired _onSessionTreadExpired = null;

        static public UpdateUiDelegate UiUpdater { set { _this._UiUpdater = value; } }
        static public RequestBeginsDelegate OnRequestBegin { set { _this._onRequestBegin = value; } }
        static public RequestEndDelegate OnRequestEnd { set { _this._onRequestEnd = value; } }
        static public RequestFailedDelegate OnRequestFailed { set { _this._onRequestFailed = value; } }
        static public SessionTreadExpired OnSessionTreadExpired { set { _this._onSessionTreadExpired = value; } }

        private int _requestTimeoutSec;
        private string _sid;

        public static string SID { get { return _this._sid; } }
        private int _keepalivePeriodSec;
        private ManualResetEvent _threadQuit = new ManualResetEvent(false);

        static private ServerProxy _this = null;

        static public bool Create(string serverEndpoint, int requestTimeoutSec)
        {
            if (_this != null) _this.Dispose();
            _this = new ServerProxy(serverEndpoint, requestTimeoutSec);
            return true;
        }

        private ServerProxy(string serverEndpoint, int requestTimeoutSec)
        {
            ServerEndpoint = serverEndpoint;
            _requestTimeoutSec = requestTimeoutSec;
            _sid = null;
            SessionLooseDescription = string.Empty;
        }

        public void Dispose()
        {
            _threadQuit.Set();
        }

        static public bool Online { get { lock (_this._threadQuit) return (_this._sid != null); } }
        static public string SessionLooseDescription { get; private set; }
        static public string ServerEndpoint { get; set; }

        private void keepaliveThread()
        {
            while (!_threadQuit.WaitOne(_keepalivePeriodSec * 1000))
            {
                if (null == _sid) break;  // if SID was disposed (logout etc.) exit immediately

                ServerResponse response = null;

                try
                {
                    response = makeProgramCall("q=sessionrenew");
                }
                catch (Exception ex)
                {
                    // ex = {"Cross-thread operation not valid: Control 'MainForm' accessed from a thread other than the thread it was created on."}
                    lock (_threadQuit)
                    {
                        if (_onSessionTreadExpired != null)
                            _onSessionTreadExpired();
                        _sid = null;
                        SessionLooseDescription = "Server not reachable (" + ex.Message + ")";
                    }
                    break;
                }

                if (HttpStatusCode.OK != response.ResponseCode)
                {
                    lock (_threadQuit)
                    {
                        _sid = null;
                        SessionLooseDescription = response.ResponseCodeDescription;
                    }
                    break;
                }
            }
        }

        static public bool Test()
        {
            // http://pastebin.com/nvB4EZbn

            bool result = false;

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ServerEndpoint + "version");
            request.Timeout = _this._requestTimeoutSec * 1000;

            //request.UserAgent = "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/13.0.780.0 Safari/535.1";
            //request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            //request.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
            //request.Headers.Add("Accept-Language", "en-US,en;q=0.8");
            //request.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.3");

            try
            {
                WebResponse response = request.GetResponse();

                if (null == response)
                {
                    // no service
                }
                else
                {
                    string responseText;

                    using (StreamReader readStream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseText = readStream.ReadToEnd();
                    }

                    if (responseText.IndexOf("vr estate server", StringComparison.InvariantCultureIgnoreCase) >= 0)
                        result = true;

                    //Stream rs = response.GetResponseStream();

                    //StringBuilder result = new StringBuilder();
                    //do
                    //{
                    //    byte[] buffer = new byte[1024];
                    //    int cnt = rs.Read(buffer, 0, buffer.Length);
                    //    if (0 == cnt) break;
                    //    result.Append(Encoding.UTF8.GetString(buffer, 0, cnt));
                    //} while (true);

                    //string type = response.Headers["content-type"];

                    //if (response.ContentType.Equals("text/html"))
                    //{

                    //}
                }

                //HttpRequest request = new HttpRequest(null, "http://localhost:8000/vre/test", string.Empty);
                //request.c
            }
            catch (Exception ex)
            {
            }

            return result;
        }

        static public int MyID { private set; get; }

        static public bool SuperadminLogin(string login, string password)
        {
            bool result = false;

            ServerResponse response = _this.makeProgramCall(string.Format("q=login&uid={0}&pwd={1}",
                System.Web.HttpUtility.UrlEncode(login),
                System.Web.HttpUtility.UrlEncode(password)));

            if (HttpStatusCode.OK == response.ResponseCode)
            {
                MyID = response.Data.GetProperty("userId", 0);
                _this._keepalivePeriodSec = response.Data.GetProperty("keepalivePeriodSec", 300);

                string sid = response.Data.GetProperty("sid", string.Empty);
                if (!string.IsNullOrWhiteSpace(sid))
                {
                    lock (_this._threadQuit)
                    {
                        _this._sid = sid;
                        SessionLooseDescription = string.Empty;
                        result = true;
                    }
                    new Thread(_this.keepaliveThread) { IsBackground = true }.Start();
                }
            }

            return result;
        }

        static public bool Login(string login, string password, string role, string developerId)
        {
            bool result = false;
            string loginString = string.Format("q=login&uid={0}&pwd={1}&role={2}&ed={3}",
                System.Web.HttpUtility.UrlEncode(login),
                System.Web.HttpUtility.UrlEncode(password),
                role, developerId);
            ServerResponse response = _this.makeProgramCall(loginString);

            if (HttpStatusCode.OK == response.ResponseCode)
            {
                _this._keepalivePeriodSec = response.Data.GetProperty("keepalivePeriodSec", 300);

                string sid = response.Data.GetProperty("sid", string.Empty);
                if (!string.IsNullOrWhiteSpace(sid))
                {
                    lock (_this._threadQuit)
                    {
                        _this._sid = sid;
                        SessionLooseDescription = string.Empty;
                        result = true;
                    }
                    new Thread(_this.keepaliveThread) { IsBackground = true }.Start();
                }
            }

            return result;
        }

        static public ServerResponse MakeDataRequest(RequestType type, string path, string query, ClientData data)
        {
            if (type == RequestType.Get)
                query = query + "&genval=" + _generation.ToString();

            ServerResponse resp = _this.makeRequest(type.Method, "data/" + path, query, data);
            if (HttpStatusCode.OK == resp.ResponseCode && type == RequestType.Get)
            {
                _generation = resp.Data.GetProperty("generation", 0);
            }
            return resp;
        }

        static public ServerResponse MakeGenericRequest(RequestType type, string path, string query, ClientData data)
        {
            return _this.makeRequest(type.Method, path, query, data);
        }

        private ServerResponse makeProgramCall(string query)
        {
            return makeRequest("GET", "program", query, null);
        }

        private ServerResponse makeRequest(string method, string path, string query, ClientData data)
        {
            if (_onRequestBegin != null)
                _onRequestBegin();

            HttpStatusCode respondStatus = HttpStatusCode.Ambiguous;
            string respondDescription = "GetRequestStream failed";
            ClientData respData = null;

            try
            {
                if (_UiUpdater != null) // on UI thread
                    _UiUpdater();

                string requestUri = ServerEndpoint + path;

                if (_sid != null)
                    query += "&sid=" + _sid;

                if (!string.IsNullOrWhiteSpace(query)) requestUri += "?" + query;

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                request.Timeout = _requestTimeoutSec * 1000;

                request.Method = method;
                Trace.WriteLine("---------------------------------");
                Trace.WriteLine("request.Method=" + request.Method);

                if (data != null)
                {
                    request.ContentType = "application/json";

                    Stream output = request.GetRequestStream();

                    using (System.IO.StreamWriter w = new System.IO.StreamWriter(output))
                    {
                        string JSON = JavaScriptHelper.ClientDataToJson(data);
                        Trace.WriteLine( "JSON=" + JSON);
                        w.Write(JSON);
                    }
                }

                // TODO: restrict concurrent requests?!
                //lock (_serverEndpoint)
                {
                    HttpWebResponse response;

                    try
                    {
                        Trace.WriteLine("request.URL=" + request.Address.ToString());
                        response = request.GetResponse() as HttpWebResponse;
                    }
                    catch (WebException ex)
                    {
                        response = ex.Response as HttpWebResponse;
                    }
                    if (null == response) throw new InvalidOperationException("Http request returned invalid result.");

                    //if (response.ContentType.Equals("application/json"))  TODO !!!
                    respData = JavaScriptHelper.JsonToClientData(response.GetResponseStream());
                    if (respData != null)
                    {
                        string JSON = JavaScriptHelper.ClientDataToJson(respData);
                        Trace.WriteLine("respData=" + JSON);
                    }
                    respondStatus = response.StatusCode;
                    respondDescription = response.StatusDescription;
                }
            }
            catch (WebException webE)
            {
                respondStatus = HttpStatusCode.Ambiguous;
                respondDescription = webE.ToString();

                if (_onRequestFailed != null)
                    _onRequestFailed(respondDescription);
            }
            finally
            {
                if (_onRequestEnd != null)
                    _onRequestEnd();
            }

            return new ServerResponse(respondStatus, respondDescription, respData); ;
        }

    }
}