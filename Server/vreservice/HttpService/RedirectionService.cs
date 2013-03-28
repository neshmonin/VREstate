using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using Vre.Server.RemoteService;
using System.Reflection;
using System.Text;

namespace Vre.Server.HttpService
{
    internal class RedirectionService : HttpServiceBase
    {
        enum ServiceType { Unknown, /*Const,*/ Redirection, Button, Reverse }
        private const string _defaultImage = "default.png";

        private string _buttonStorePath;
        private string _defaultRedirectUri;
        private AliasMap _map, _testMap;
        private ButtonStoreFsNameCache _imagePathCache;
        private bool _allowReallyExtendedLogging;
        private string _clientViewOrderTemplate, _testViewOrderTemplate;
        private string _clientRRTemplate, _testRRTemplate;

        public RedirectionService() : base("HTTP Redirection")
        {
            _allowReallyExtendedLogging = _allowExtendedLogging & 
                ServiceInstances.Configuration.GetValue("DebugAllowReallyExtendedLogging", false);

            _defaultRedirectUri = ServiceInstances.Configuration.GetValue("RedirectorDefaultUri", "http://3dcondox.com");

            _buttonStorePath = ServiceInstances.Configuration.GetValue("RedirectorButtonsStore", string.Empty);
            if (string.IsNullOrWhiteSpace(_buttonStorePath))
            {
                _buttonStorePath = null;
                _imagePathCache = null;
                ServiceInstances.Logger.Error("RedirectorButtonsStore parameter is not set; buttons shall not be provided.");
            }
            else
            {
                _imagePathCache = new ButtonStoreFsNameCache(_buttonStorePath);
            }

            _map = new AliasMap(ServiceInstances.Configuration.GetValue("RedirectionAliasMapFile", "aliases.config"));
            _testMap = new AliasMap(ServiceInstances.Configuration.GetValue("RedirectionTestAliasMapFile", "aliases.test.config"));

            _clientViewOrderTemplate = ServiceInstances.Configuration.GetValue("RedirectionClientViewOrderTemplate", "https://vrt.3dcondox.com/VREstate.html?viewOrderId={0}");
            _testViewOrderTemplate = ServiceInstances.Configuration.GetValue("RedirectionTestClientViewOrderTemplate", "https://vrt.3dcondox.com/vre/VREstate.html?viewOrderId={0}");

            _clientRRTemplate = ServiceInstances.Configuration.GetValue("RedirectionRevReqTemplate", "https://vrt.3dcondox.com/go/{0}");
            _testRRTemplate = ServiceInstances.Configuration.GetValue("RedirectionTestRevReqTemplate", "https://vrt.3dcondox.com/vre/go/{0}");
        }

        protected override IResponseData process(string browserKey, HttpListenerContext ctx, HttpServiceRequest.ProcessResponse proc)
        {
            IResponseData result = new HttpServiceRequest.ResponseData(new MemoryStream(0), ctx.Response, proc);

            if (ctx.Request.HttpMethod.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
            {
                if (_allowExtendedLogging)
                {
                    if (_allowReallyExtendedLogging)
                        ServiceInstances.RequestLogger.Info("{0}: {1} {2} {3} {4} {5} {6}", browserKey,
                            ctx.Request.Url, ctx.Request.RemoteEndPoint, ctx.Request.UserAgent,
                            ctx.Request.UserHostAddress, ctx.Request.UserHostName, ctx.Request.UrlReferrer);
                    else
                        ServiceInstances.RequestLogger.Info("{0}: {1}", browserKey, ctx.Request.Url);
                }

                string path = ctx.Request.Url.LocalPath;
                if (path.StartsWith(_path))
                {
                    path = path.Remove(0, _path.Length);
                    if (path.Length > 0)
                    {
                        switch (serviceTypeByName(path))
                        {
                            case ServiceType.Redirection:
                                result.RedirectionUrl = redirect(ctx, browserKey);
                                break;

                            case ServiceType.Reverse:
                                result.RedirectionUrl = reverse(ctx, browserKey);
                                break;

                            case ServiceType.Button:
                                processButtonRequest(ctx, browserKey, result);
                                break;

                            //case ServiceType.Const:
                            //    processConst(ctx, path, result);
                            //    break;

                            default:
                                ProcessFileRequest(path, result);
                                break;
                        }
                    }
                    else  // path is empty; test or error; redirect to main site
                    {
                        result.RedirectionUrl = _defaultRedirectUri;
                    }
                }
                else
                {
                    result.RedirectionUrl = _defaultRedirectUri;
                }
            }
            else
            {
                result.ResponseCode = HttpStatusCode.NotImplemented;
            }

            return result;
        }

        private static ServiceType serviceTypeByName(string pathElement)
        {
            if (pathElement.Equals("start")) return ServiceType.Redirection;
            else if (pathElement.Equals("button")) return ServiceType.Button;
            else if (pathElement.Equals("go")) return ServiceType.Reverse;
            //else if (pathElement.Equals("robots.txt")) return ServiceType.Const;
            //else if (pathElement.Equals("humans.txt")) return ServiceType.Const;
            //else if (pathElement.Equals("version")) return ServiceType.Const;
            return ServiceType.Unknown;
        }

        private string redirect(HttpListenerContext ctx, string browserKey)
        {
            string finalUri = null;
            bool testMode = false;

            foreach (string k in ctx.Request.QueryString.AllKeys)
            {
                if (k.Equals("project"))
                {
                    finalUri = ctx.Request.QueryString[k];
                }
                else if (k.Equals("test"))
                {
                    testMode = ctx.Request.QueryString[k].Equals("true");
                }
            }

            if (finalUri != null) finalUri = testMode ? _testMap.UriByAlias(finalUri) : _map.UriByAlias(finalUri);
            if (null == finalUri) finalUri = _defaultRedirectUri;

            string queryString = ctx.Request.Url.Query;
            if (queryString.Length > 0)
            {
                if (finalUri.Contains("?")) finalUri += "&" + queryString.Substring(1);
                else finalUri += queryString;
            }

            //ctx.Request.UrlReferrer;
            // TODO: save statistics
            //ctx.Response.Redirect(finalUri);
            return finalUri;
        }

        private string reverse(HttpListenerContext ctx, string browserKey)
        {
            bool testMode = false;
            string id = null;

            foreach (string k in ctx.Request.QueryString.AllKeys)
            {
                if (k.Equals("id"))
                {
                    id = ctx.Request.QueryString[k];
                }
                else if (k.Equals("test"))
                {
                    testMode = ctx.Request.QueryString[k].Equals("true");
                }
            }

            string finalUri;

            if (null == id)
            {
                finalUri = _defaultRedirectUri;
            }
            else
            {
                switch (UniversalId.TypeInUrlId(id))
                {
                    default:  // legacy
                    case UniversalId.IdType.ViewOrder:
                        finalUri = string.Format(testMode ? _testViewOrderTemplate : _clientViewOrderTemplate, id);
                        break;

                    case UniversalId.IdType.ReverseRequest:
                        finalUri = string.Format(testMode ? _testRRTemplate : _clientRRTemplate, id);
                        break;
                }
            }

            //ctx.Request.UrlReferrer;
            // TODO: save statistics
            //ctx.Response.Redirect(finalUri);
            return finalUri;
        }

        private void processButtonRequest(HttpListenerContext ctx, string browserKey, IResponseData response)
        {
            string aliasName = null;
            string imageName = "default";

            foreach (string k in ctx.Request.QueryString.AllKeys)
            {
                if (k.Equals("project"))
                {
                    aliasName = ctx.Request.QueryString[k];
                    if (null == _map.UriByAlias(aliasName)) aliasName = null;
                }
                else if (k.Equals("image"))
                {
                    imageName = ctx.Request.QueryString[k];
                }
            }
            
            bool served = false;

            if (aliasName != null)
            {
                if (!IsPathValid(imageName, false)) imageName = "default";
                
                string fullPath = (_imagePathCache != null) ? _imagePathCache.PathByHint(aliasName, imageName) : null;

                if (fullPath != null)
                {
                    response.DataPhysicalLocation = fullPath;
                    streamImage(fullPath, response);
                    served = true;
                }
            }

            if (!served)
                streamImage(Path.Combine(_buttonStorePath, _defaultImage), response);

            //ctx.Request.UrlReferrer;
            // TODO: save statistics

        }

        //private static void processConst(HttpListenerContext ctx, string path, IResponseData response)
        //{
        //    response.ResponseCode = HttpStatusCode.OK;
        //    response.DataStreamContentType = "txt";

        //    StringBuilder text = new StringBuilder();

        //    if (path.Equals("robots.txt"))
        //    {
        //        text.Append("User-agent: *\r\n");
        //        text.Append("Disallow: /\r\n");
        //        text.Append("Disallow: /harming/humans\r\n");
        //        text.Append("Disallow: /ignoring/human/orders\r\n");
        //        text.Append("Disallow: /harm/to/self\r\n");
        //    }
        //    else if (path.Equals("humans.txt"))
        //    {
        //        if (DateTime.MinValue == _buildTime)
        //        {
        //            _buildTime = File.GetCreationTime(Assembly.GetExecutingAssembly().Location);
        //        }

        //        text.Append("/* humanstxt.org */\r\n");
        //        text.Append("\r\n");
        //        text.Append("/* TEAM */\r\n");
        //        text.Append("\tAlexander Neshmonin, CEO and everything, Toronto\r\n");
        //        text.Append("\tEugene Simonov, Frontend, Ukraine\r\n");
        //        text.Append("\tAndrey Maslyuk, Backend, Toronto\r\n");
        //        text.Append("\r\n");
        //        text.Append("/* THANKS */\r\n");
        //        text.Append("\tVitaly Zholudev\r\n");
        //        text.Append("\thttp://last.fm\r\n");
        //        text.Append("\thttp://stackoverflow.com\r\n");
        //        text.Append("\r\n");
        //        text.Append("\r\n");
        //        text.Append("/* SITE */\r\n");
        //        text.Append("\tLast build: " + _buildTime.ToShortDateString() + "\r\n");
        //        text.Append("\tLast update: today\r\n");
        //        text.Append("\tStandards: HTTP/1.1\r\n");
        //        text.Append("\tLanguages: none\r\n");
        //    }
        //    else if (path.Equals("version"))
        //    {
        //        text.Append(RemoteServiceProvider.BuildVersionInformation());
        //    }

        //    byte[] buffer = Encoding.UTF8.GetBytes(text.ToString());
        //    response.DataStream.Write(buffer, 0, buffer.Length);
        //}

        //private string deriveExtension(string aliasName, string imageName)
        //{
        //    string result = null;

        //    try
        //    {
        //        foreach (string file in Directory.EnumerateFiles(
        //            Path.Combine(_buttonStorePath, aliasName), imageName + ".*", SearchOption.TopDirectoryOnly))
        //        {
        //            result = Path.GetExtension(file);
        //            break;
        //        }
        //    }
        //    catch (DirectoryNotFoundException) { }

        //    return result;
        //}

        private static void streamImage(string path, IResponseData response)
        {
            response.ResponseCode = HttpStatusCode.OK;
            response.DataStreamContentType = Path.GetExtension(path).ToLower().Substring(1);
            response.DataPhysicalLocation = path;
        }
    }

    internal class ButtonStoreFsNameCache
    {
        private static Dictionary<string, string> _cache = new Dictionary<string, string>();
        private FileSystemWatcher _watcher;
        private string _path;

        public ButtonStoreFsNameCache(string path)
        {
            _path = path;
            if (_path.EndsWith(new string(Path.DirectorySeparatorChar, 1))) _path = _path.Substring(0, _path.Length - 1);

            _watcher = null;
            initializeWatcher();
        }

        public string PathByHint(string aliasName, string imageName)
        {
            string result;
            string hint = Path.Combine(aliasName, imageName);
            lock (_cache)
            {
                if (!_cache.TryGetValue(hint, out result))
                {
                    result = deriveExtension(aliasName, imageName);
                    _cache.Add(hint, result);
                }
            }
            return result;
        }

        private string deriveExtension(string aliasName, string imageName)
        {
            string result = null;

            try
            {
                foreach (string file in Directory.EnumerateFiles(
                    Path.Combine(_path, aliasName), imageName + ".*", SearchOption.TopDirectoryOnly))
                {
                    result = file;
                    break;
                }
            }
            catch (DirectoryNotFoundException) { }

            return result;
        }

        private void initializeWatcher()
        {
            lock (this)
            {
                if (_watcher != null) return;

                _watcher = new FileSystemWatcher(_path);
                _watcher.Filter = "*.*";
                _watcher.IncludeSubdirectories = true;
                _watcher.EnableRaisingEvents = false;

                //_watcher.Changed += new FileSystemEventHandler(_watcher_CCD);
                _watcher.Created += new FileSystemEventHandler(_watcher_Created);
                _watcher.Deleted += new FileSystemEventHandler(_watcher_Deleted);
                _watcher.Renamed += new RenamedEventHandler(_watcher_Renamed);
                _watcher.Error += new ErrorEventHandler(_watcher_Error);

                _watcher.EnableRaisingEvents = true;
            }
        }

        private void _watcher_Error(object sender, ErrorEventArgs e)
        {
            lock (sender)
            {
                ServiceInstances.Logger.Error("BFS: File system watcher failed.  Recreating.\r\n{0}", e.GetException());
                try { _watcher.Dispose(); }
                catch { }
                _watcher = null;
                initializeWatcher();
            }
        }

        private void _watcher_Created(object sender, FileSystemEventArgs e)
        {
            lock (sender)
            {
                ServiceInstances.Logger.Info("BSC: Detected added file: \"{0}\"", e.FullPath);
                processAdd(e.FullPath);
            }
        }

        private void _watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            lock (sender)
            {
                ServiceInstances.Logger.Info("BSC: Detected file removal: \"{0}\"", e.FullPath);
                processDelete(e.FullPath);
            }
        }

        private void _watcher_Renamed(object sender, RenamedEventArgs e)
        {
            lock (sender)
            {
                ServiceInstances.Logger.Info("BSC: Detected file rename: \"{0}\" -> \"{1}\"", e.OldFullPath, e.FullPath);
                processDelete(e.OldFullPath);
                processAdd(e.FullPath);
            }
        }

        private void processAdd(string fullPath)
        {
            string fullPathNoExtension = Path.Combine(Path.GetDirectoryName(fullPath),
                Path.GetFileNameWithoutExtension(fullPath));
            if (fullPathNoExtension.StartsWith(_path, StringComparison.InvariantCultureIgnoreCase))
            {
                string createdHint = fullPathNoExtension.Substring(_path.Length + 1);
                lock (_cache)
                {
                    string foundKey = null;
                    foreach (string hint in _cache.Keys)
                    {
                        if (hint.Equals(createdHint, StringComparison.InvariantCultureIgnoreCase))
                        {
                            foundKey = hint;
                            break;
                        }
                    }
                    if (foundKey != null)
                    {
                        if (null == _cache[foundKey]) _cache[foundKey] = fullPath;
                        // otherwise it is a duplicate with a different extension
                    }
                }
            }
        }

        private void processDelete(string fullPath)
        {
            lock (_cache)
            {
                string foundKey = null;
                foreach (KeyValuePair<string, string> kvp in _cache)
                {
                    if (null == kvp.Value) continue;
                    if (kvp.Value.Equals(fullPath, StringComparison.InvariantCultureIgnoreCase))
                    {
                        foundKey = kvp.Key;
                        break;
                    }
                }
                if (foundKey != null)
                {
                    string[] path = foundKey.Split(Path.DirectorySeparatorChar);
                    if (2 == path.Length)
                    {
                        // try looking for other files with same name and different extension
                        _cache[foundKey] = deriveExtension(path[0], path[1]);
                    }
                    else  // dunno what to do!
                    {
                        _cache[foundKey] = null;
                        ServiceInstances.Logger.Error("BSC: Found an unknowk cache key type: '{0}'", foundKey);
                    }
                }
            }
        }
    }

    internal class AliasMap
    {
        private FileSystemWatcher _watcher;
        private string _filePath;
        private Dictionary<string, string> _map;

        public AliasMap(string fileName)
        {
            _filePath = fileName;

            if (!Path.IsPathRooted(_filePath)) 
                _filePath = Path.Combine(Path.GetDirectoryName(ServiceInstances.Configuration.FilePath), _filePath);

            _map = null;
            _watcher = null;
            initializeWatcher();
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

        private void initializeWatcher()
        {
            lock (this)
            {
                if (_watcher != null) return;

                _watcher = new FileSystemWatcher(Path.GetDirectoryName(_filePath));
                _watcher.Filter = Path.GetFileName(_filePath);
                _watcher.IncludeSubdirectories = false;
                _watcher.EnableRaisingEvents = false;

                _watcher.Changed += new FileSystemEventHandler(_watcher_Changed);
                _watcher.Created += new FileSystemEventHandler(_watcher_Created);
                _watcher.Deleted += new FileSystemEventHandler(_watcher_Deleted);
                _watcher.Error += new ErrorEventHandler(_watcher_Error);

                ServiceInstances.Logger.Info("AC: Started reading alias map.");
                reReadMap();
                ServiceInstances.Logger.Info("AC: Reading alias map done.");

                _watcher.EnableRaisingEvents = true;
            }
        }

        private void _watcher_Error(object sender, ErrorEventArgs e)
        {
            lock (this)
            {
                ServiceInstances.Logger.Error("AC: File system watcher failed.  Recreating.\r\n{0}", e.GetException());
                try { _watcher.Dispose(); }
                catch { }
                _watcher = null;
                initializeWatcher();
            }
        }

        private void _watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            lock (this)
            {
                ServiceInstances.Logger.Info("AC: Detected removal of alias map: \"{0}\"", e.FullPath);
                // NOOP; cache?
            }
        }

        private void _watcher_Created(object sender, FileSystemEventArgs e)
        {
            lock (this)
            {
                ServiceInstances.Logger.Info("AC: Started reading alias map.");
                reReadMap();
                ServiceInstances.Logger.Info("AC: Reading alias map done.");
            }
        }

        private void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            lock (this)
            {
                ServiceInstances.Logger.Info("AC: Started reading alias map.");
                reReadMap();
                ServiceInstances.Logger.Info("AC: Reading alias map done.");
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