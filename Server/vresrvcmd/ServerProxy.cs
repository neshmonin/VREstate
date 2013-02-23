using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Vre.Server;
using Vre.Server.BusinessLogic;

namespace Vre.Client
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
        public class RequestType
        {
            public string Method { get; private set; }
            private RequestType(string method) { Method = method; }
            public static RequestType Get { get { return new RequestType("GET"); } }
            public static RequestType Insert { get { return new RequestType("POST"); } }
            public static RequestType Update { get { return new RequestType("PUT"); } }
            public static RequestType Delete { get { return new RequestType("DELETE"); } }
        }

        private Control _referenceControl;
        private int _requestTimeoutSec;
        private string _sid;
        private int _keepalivePeriodSec;
        private ManualResetEvent _threadQuit = new ManualResetEvent(false);

        public ServerProxy(string serverEndpoint, int requestTimeoutSec, Control referenceControl)
        {
            ServerEndpoint = serverEndpoint;
            _requestTimeoutSec = requestTimeoutSec;
            _sid = null;
            SessionLooseDescription = string.Empty;
            _referenceControl = referenceControl;
            LoginId = string.Empty;
        }

        public void Dispose()
        {
            _threadQuit.Set();
        }

        public bool Online { get { lock (_threadQuit) return (_sid != null); } }
        public string LoginId { get; private set; }
        public string SessionLooseDescription { get; private set; }
        public string ServerEndpoint { get; private set; }

        private void keepaliveThread()
        {
            Thread.CurrentThread.Name = "SessionKeepalive#" + Thread.CurrentThread.ManagedThreadId.ToString();

            while (!_threadQuit.WaitOne(_keepalivePeriodSec * 1000))
            {
                if (null == _sid) break;  // if SID was disposed (logout etc.) exit immediately

                ServerResponse response = null;

                try
                {
                    response = MakeProgramCall("q=sessionrenew");
                }
                catch (Exception ex)
                {
                    lock (_threadQuit)
                    {
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

        public bool Test()
        {
            // http://pastebin.com/nvB4EZbn

            bool result = false;

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ServerEndpoint + "version");
            request.Timeout = _requestTimeoutSec * 1000;

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

        public bool Login(string login, string password)
        {
            bool result = false;

            ServerResponse response = MakeProgramCall(string.Format("q=login&uid={0}&pwd={1}",
                System.Web.HttpUtility.UrlEncode(login),
                System.Web.HttpUtility.UrlEncode(password)));

            if (HttpStatusCode.OK == response.ResponseCode)
            {
                _keepalivePeriodSec = response.Data.GetProperty("keepalivePeriodSec", 30);

                string sid = response.Data.GetProperty("sid", string.Empty);
                if (!string.IsNullOrWhiteSpace(sid))
                {
                    lock (_threadQuit)
                    {
                        _sid = sid;
                        LoginId = login;
                        SessionLooseDescription = string.Empty;
                        result = true;
                    }
                    new Thread(keepaliveThread) { IsBackground = true }.Start();
                }
            }

            return result;
        }

        public ServerResponse MakeRestRequest(RequestType type, string path, string query, ClientData data)
        {
            return makeRequest(type.Method, "data/" + path, query, data);
        }

        public ServerResponse MakeProgramCall(string query)
        {
            return makeRequest("GET", "program", query, null);
        }

        private ServerResponse makeRequest(string method, string path, string query, ClientData data)
        {
            Cursor saved = Cursor.Current;

            try
            {
                if ((_referenceControl != null) && !_referenceControl.InvokeRequired) // on UI thread
                {
                    Cursor.Current = Cursors.WaitCursor;
                    _referenceControl.Update();
                }

                string requestUri = ServerEndpoint + path;

                if (!string.IsNullOrWhiteSpace(query)) requestUri += "?" + query;

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                request.Timeout = _requestTimeoutSec * 1000;

                request.Method = method;

                if (_sid != null) request.Headers.Add("sid", _sid);

                if (data != null)
                {
                    request.ContentType = "application/json";

                    Stream output = request.GetRequestStream();

                    using (System.IO.StreamWriter w = new System.IO.StreamWriter(output))
                        w.Write(JavaScriptHelper.ClientDataToJson(data));
                }

                // TODO: restrict concurrent requests?!
                //lock (_serverEndpoint)
                {
                    HttpWebResponse response;

                    try
                    {
                        response = request.GetResponse() as HttpWebResponse;
                    }
                    catch (WebException ex)
                    {
                        response = ex.Response as HttpWebResponse;
                    }
                    if (null == response) throw new InvalidOperationException("Http request returned invalid result.");

                    ClientData respData = null;
                    //if (response.ContentType.Equals("application/json"))  TODO !!!
                    respData = JavaScriptHelper.JsonToClientData(response.GetResponseStream());

                    return new ServerResponse(response.StatusCode, response.StatusDescription, respData);
                }
            }
            finally
            {
                Cursor.Current = saved;
            }
        }

    }
}