using System;
using NLog;
using Vre.Server.RemoteService;
using Vre.Server.ModelCache;

namespace Vre.Server
{
    internal class ServiceInstances
    {
        private static readonly object _lock = new object();

        public static ModelCacheManager ModelCache;

        public static FileCacheManager FileCache;

        private static Config _configuration = null;
        public static Config Configuration
        {
            get
            {
                lock (_lock)
                {
                    if (null == _configuration) _configuration = new Config();
                }
                return _configuration;
            }
        }

        private static Logger _logger = null;
        public static Logger Logger
        {
            get
            {
                lock (_lock)
                {
                    if (null == _logger) _logger = LogManager.GetLogger("Vre.Server.ServiceInstance");
                }
                return _logger;
            }
        }

        private static Logger _requestLogger = null;
        public static Logger RequestLogger
        {
            get
            {
                lock (_lock)
                {
                    if (null == _requestLogger) _requestLogger = LogManager.GetLogger("Vre.Server.Request");
                }
                return _requestLogger;
            }
        }

        private static FloodPreventor _floodPreventor = null;
        public static FloodPreventor FloodStopper
        {
            get
            {
                lock (_lock)
                {
                    if (null == _floodPreventor) _floodPreventor = new FloodPreventor();
                }
                return _floodPreventor;
            }
        }

        private static ClientSessionStore _clientSessionStore = null;
        public static ClientSessionStore SessionStore
        {
            get
            {
                lock (_lock)
                {
                    if (null == _clientSessionStore) _clientSessionStore = new ClientSessionStore();
                }
                return _clientSessionStore;
            }
        }

        private static Spikes.PullUpdateService _pullUpdateService = null;
        public static Spikes.PullUpdateService UpdateService
        {
            get
            {
                lock (_lock)
                {
                    if (null == _pullUpdateService) _pullUpdateService = new Spikes.PullUpdateService();
                }
                return _pullUpdateService;
            }
        }

        public static void Dispose()
        {
            lock (_lock)
            {
                if (null != _floodPreventor) { _floodPreventor.Dispose(); _floodPreventor = null; }
                if (null != _clientSessionStore) { _clientSessionStore.Dispose(); _clientSessionStore = null; }
            }
        }
    }

    //public interface ILogger
    //{
    //    void Info(string text);
    //    void Info(string text, params object[] args);
    //    void Debug(string text);
    //    void Debug(string text, params object[] args);
    //    void Warn(string text);
    //    void Warn(string text, params object[] args);
    //    void Error(string text);
    //    void Error(string text, params object[] args);
    //    void Fatal(string text);
    //    void Fatal(string text, params object[] args);
    //}
}