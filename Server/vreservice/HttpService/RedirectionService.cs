using System;
using System.Collections.Generic;
using System.Net;
using Vre.Server.RemoteService;
using System.Threading;
using System.IO;
using System.Xml;
using Vre.Server.HttpService;

namespace Vre.Server.Redirection.HttpService
{
    internal class HttpService
    {
        enum ServiceType { Unknown, Redirection, Button }
        private const string _defaultImage = "default.png";
        private const string _defaultImageExtension = ".png";

        private static readonly object _lock = new object();
        private static List<string> _listeners = new List<string>();

        public static string Status = "Stopped.";
        public static string[] Listeners { get { lock (_listeners) return _listeners.ToArray(); } }

        private static HttpListener _httpListener;

        private static int _fileBufferSize = 16384;
        private static string _path;
        private static string _buttonStorePath;
        private static string _defaultRedirectUri;
        private static AliasMap _map;
        private static bool _allowExtendedLogging;

        private static Dictionary<string, string> _imagePathCache = new Dictionary<string, string>();

        public static void PerformStartup()
        {
            ServiceInstances.Logger.Info("Starting HTTP redirection service.");

            _httpListener = new HttpListener();

            _allowExtendedLogging = ServiceInstances.Configuration.GetValue("DebugAllowExtendedLogging", false);

            _defaultRedirectUri = ServiceInstances.Configuration.GetValue("RedirectorDefaultUri", "http://3dcondox.com");

            _fileBufferSize = ServiceInstances.Configuration.GetValue("FileStreamingBufferSize", 16384);

            _buttonStorePath = ServiceInstances.Configuration.GetValue("RedirectorButtonsStore", string.Empty);
            if (string.IsNullOrWhiteSpace(_buttonStorePath))
            {
                _buttonStorePath = null;
                ServiceInstances.Logger.Error("RedirectorButtonsStore parameter is not set; buttons shall not be provided.");
            }

            _map = new AliasMap();

            string uriText = ServiceInstances.Configuration.GetValue("HttpListenerUri", string.Empty);
            if (string.IsNullOrEmpty(uriText))
            {
                ServiceInstances.Logger.Warn("HTTP service listening point is not set. HTTP service is not started.");
                return;
            }

            ServiceInstances.Logger.Info("HTTP Redirection Service is starting:");

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

            ServiceInstances.Logger.Info("HTTP Redirection Service started.");

            if (_allowExtendedLogging)
                ServiceInstances.RequestLogger.Info("HTTP Redirection Service started");
        }

        public static void PerformShutdown()
        {
            if (null != _httpListener)
            {
                ServiceInstances.Logger.Info("HTTP Redirection Service is stopping.");
                _httpListener.Stop();
                _httpListener.Close();
                _httpListener = null;
                ServiceInstances.Logger.Info("HTTP Redirection Service stopped.");
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
                    if (ctx.Request.HttpMethod.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (_allowExtendedLogging)
                            ServiceInstances.RequestLogger.Info(ctx.Request.Url);

                        Cookie browserId = ctx.Request.Cookies["a"];
                        string browserKey;

                        if (null == browserId)
                        {
                            browserId = new Cookie("a", Guid.NewGuid().ToString());
                            browserId.Expires = DateTime.Now.AddYears(10);
                            ctx.Response.Cookies.Add(browserId);
                        }
                        browserKey = browserId.Value;

                        string path = ctx.Request.Url.LocalPath;
                        if (path.StartsWith(_path))
                        {
                            path = path.Remove(0, _path.Length);

                            string finalUri = _defaultRedirectUri;
                            string[] pathElements = path.Split('/');
                            if (pathElements.Length >= 2)
                            {
                                ServiceType st = serviceTypeByName(pathElements[0]);
                                string uriBase = _map.UriByAlias(pathElements[1]);
                                if (uriBase != null)
                                {
                                    switch (st)
                                    {
                                        case ServiceType.Redirection:
                                            if (2 == pathElements.Length)
                                            {
                                                string q = ctx.Request.Url.Query;
                                                if (string.IsNullOrWhiteSpace(q)) finalUri = uriBase;
                                                else
                                                {
                                                    if (q.StartsWith("?")) q = q.Substring(1);
                                                    finalUri = uriBase + "&" + q;
                                                }
                                            }
                                            break;

                                        case ServiceType.Button:
                                            if (3 == pathElements.Length)
                                            {
                                                processButtonRequest(pathElements[1], pathElements[2], ctx.Response);
                                                finalUri = null;
                                            }
                                            else
                                            {
                                                streamImage(Path.Combine(_buttonStorePath, _defaultImage), 
                                                    _defaultImageExtension, ctx.Response);
                                            }
                                            break;
                                    }
                                }
                            }
                            //ctx.Request.UrlReferrer;
                            // TODO: save statistics
                            
                            if (finalUri != null) ctx.Response.Redirect(finalUri);
                        }
                        else
                        {
                            //ctx.Request.UrlReferrer;
                            // TODO: save statistics

                            ctx.Response.Redirect(_defaultRedirectUri);
                        }
                    }
                    else
                    {
                        ctx.Response.StatusCode = 501;
                    }

                    ctx.Response.Close();
                }
                catch (HttpListenerException ex)  // these seem to be useless; just flooding logs
                {
                    ServiceInstances.Logger.Error("HTTP request processing failed: {0}", ex.Message);
                }
                catch (Exception ex)
                {
                    ctx.Response.StatusCode = 500;
                    ctx.Response.StatusDescription = "Server error.";
                    ctx.Response.Close();
                    ServiceInstances.Logger.Error("HTTP request processing failed: {0}", ex);
                }
            }
        }

        private static ServiceType serviceTypeByName(string pathElement)
        {
            if (pathElement.Equals("start")) return ServiceType.Redirection;
            else if (pathElement.Equals("button")) return ServiceType.Button;
            return ServiceType.Unknown;
        }

        private static void processButtonRequest(string aliasName, string imageName, HttpListenerResponse response)
        {
            string pathElement = aliasName + Path.DirectorySeparatorChar + imageName;
            string extension;
            lock (_imagePathCache) if (!_imagePathCache.TryGetValue(pathElement, out extension)) extension = null;
            if (null == extension)
            {
                extension = deriveExtension(aliasName, imageName);
                if (extension != null)
                    lock (_imagePathCache)
                        if (!_imagePathCache.ContainsKey(pathElement))
                            _imagePathCache.Add(pathElement, extension);
            }

            if (extension != null)
            {
                try
                {
                    streamImage(Path.Combine(_buttonStorePath, pathElement) + extension, extension, response);
                }
                catch (FileNotFoundException)
                {
                    extension = null;
                }
            }

            if (null == extension)
                streamImage(Path.Combine(_buttonStorePath, _defaultImage), _defaultImageExtension, response);
        }

        private static string deriveExtension(string aliasName, string imageName)
        {
            string result = null;

            try
            {
                foreach (string file in Directory.EnumerateFiles(
                    Path.Combine(_buttonStorePath, aliasName), imageName + ".*", SearchOption.TopDirectoryOnly))
                {
                    result = Path.GetExtension(file);
                    break;
                }
            }
            catch (DirectoryNotFoundException) { }

            return result;
        }

        private static void streamImage(string path, string extension, HttpListenerResponse response)
        {
            string type;
            if (!HttpServiceRequest.ContentTypeByExtension.TryGetValue(extension.Substring(1), out type))
                throw new InvalidDataException("Image file type in not known.");
            
            response.StatusCode = 200;
            response.ContentType = type;

            // stream file to response
            byte[] buffer = new byte[_fileBufferSize];
            using (Stream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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

    internal class AliasMap
    {
        private string _filePath;
        private Dictionary<string, string> _map;

        public AliasMap()
        {
            _filePath = ServiceInstances.Configuration.GetValue("RedirectionAliasMapFile", string.Empty);
            if (string.IsNullOrEmpty(_filePath)) _filePath = "aliases.config";
            if (!Path.IsPathRooted(_filePath)) 
                _filePath = Path.Combine(Path.GetDirectoryName(ServiceInstances.Configuration.FilePath), _filePath);

            _map = null;
            reReadMap();

            // TODO: implement auto re-read
        }

        public string UriByAlias(string alias)
        {
            lock (this) 
            {
                string result;
                if (!_map.TryGetValue(alias, out result)) result = null;
                return result;
            }
        }

        private void reReadMap()
        {
            Dictionary<string, string> newMap = new Dictionary<string, string>();
            bool result = false;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(_filePath);

                XmlNode rootNode = doc.FirstChild;
                while (rootNode != null)
                {
                    if (rootNode.Name.Equals("configuration")) break;
                    rootNode = rootNode.NextSibling;
                }

                if (rootNode != null)
                {
                    XmlNode node = rootNode.FirstChild;

                    while (node != null)
                    {
                        if (node.Name.Equals("alias"))
                        {
                            XmlAttribute attr;
                            string name = null, uri = null;

                            attr = node.Attributes["name"];
                            if (attr != null) name = attr.Value;

                            attr = node.Attributes["uri"];
                            if (attr != null) uri = attr.Value;

                            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(uri))
                                newMap.Add(name, uri);
                        }

                        node = node.NextSibling;
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                ServiceInstances.Logger.Error("Cannot read alias map file ({0}): {1}", _filePath, ex);
            }

            lock (this) 
            {
                if (result) _map = newMap;
                else if (null == _map) _map = new Dictionary<string, string>();
            }
        }
    }
}