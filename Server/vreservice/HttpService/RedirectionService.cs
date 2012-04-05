using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using Vre.Server.RemoteService;

namespace Vre.Server.HttpService
{
    internal class RedirectionService
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
        private static ButtonStoreFsNameCache _imagePathCache;
        private static bool _allowExtendedLogging;

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
                _imagePathCache = null;
                ServiceInstances.Logger.Error("RedirectorButtonsStore parameter is not set; buttons shall not be provided.");
            }
            else
            {
                _imagePathCache = new ButtonStoreFsNameCache(_buttonStorePath);
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
                        string browserKey = Statistics.GetBrowserId(ctx);

                        if (_allowExtendedLogging)
                            ServiceInstances.RequestLogger.Info("{0}: {1}", browserKey, ctx.Request.Url);

                        string path = ctx.Request.Url.LocalPath;
                        if (path.StartsWith(_path))
                        {
                            path = path.Remove(0, _path.Length);

                            switch (serviceTypeByName(path))
                            {
                                case ServiceType.Redirection:
                                    redirect(ctx, browserKey);
                                    break;

                                case ServiceType.Button:
                                    processButtonRequest(ctx, browserKey);
                                    break;

                                default:
                                    ctx.Response.Redirect(_defaultRedirectUri);
                                    break;
                            }
                        }
                        else
                        {
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

        private static void redirect(HttpListenerContext ctx, string browserKey)
        {
            string finalUri = _defaultRedirectUri;

            foreach (string k in ctx.Request.QueryString.AllKeys)
            {
                if (k.Equals("project"))
                {
                    finalUri = _map.UriByAlias(ctx.Request.QueryString[k]);
                    if (null == finalUri) finalUri = _defaultRedirectUri;
                    break;
                }
            }

            string queryString = ctx.Request.Url.Query;
            if (queryString.Length > 0)
            {
                if (finalUri.Contains("?")) finalUri += "&" + queryString.Substring(1);
                else finalUri += queryString;
            }

            //ctx.Request.UrlReferrer;
            // TODO: save statistics

            ctx.Response.Redirect(finalUri);
        }

        private static void processButtonRequest(HttpListenerContext ctx, string browserKey)
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
                if (k.Equals("image"))
                {
                    imageName = ctx.Request.QueryString[k];
                }
            }
            
            bool served = false;

            if (aliasName != null)
            {
                if (!RemoteServiceProvider.IsPathValid(imageName, false)) imageName = "default";
                
                string fullPath = (_imagePathCache != null) ? _imagePathCache.PathByHint(aliasName, imageName) : null;

                if (fullPath != null)
                {
                    streamImage(fullPath, Path.GetExtension(fullPath), ctx.Response);
                    served = true;
                }
            }

            if (!served)
                streamImage(Path.Combine(_buttonStorePath, _defaultImage), _defaultImageExtension, ctx.Response);

            //ctx.Request.UrlReferrer;
            // TODO: save statistics

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

        public AliasMap()
        {
            _filePath = ServiceInstances.Configuration.GetValue("RedirectionAliasMapFile", string.Empty);
            if (string.IsNullOrEmpty(_filePath)) _filePath = "aliases.config";
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