using System;
using NLog;
using Vre.Server.RemoteService;
using Vre.Server.ModelCache;
using Vre.Server.FileStorage;
using Vre.Server.Messaging;
using Vre.Server.UpdateTracking;

namespace Vre.Server
{
    internal class ServiceInstances
    {
        private static readonly object _lock = new object();

        public static ModelCacheManager ModelCache;

        public static FileCacheManager FileCache;

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

		private static Logger _paymentLogger = null;
		public static Logger PaymentLogger
		{
			get
			{
				lock (_lock)
				{
					if (null == _paymentLogger) _paymentLogger = LogManager.GetLogger("Vre.Server.Payment");
				}
				return _paymentLogger;
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

        private static IFileStorageManager _fileStorageManager = null;
        public static IFileStorageManager FileStorageManager
        {
            get
            {
                lock (_lock)
                {
                    if (null == _fileStorageManager) _fileStorageManager = new LocalFileStorageManager();
                }
                return _fileStorageManager;
            }
        }

        private static IFileStorageManager _internalFileStorageManager = null;
        public static IFileStorageManager InternalFileStorageManager
        {
            get
            {
                lock (_lock)
                {
                    if (null == _internalFileStorageManager) _internalFileStorageManager = new IntLocalFileStorageManager();
                }
                return _internalFileStorageManager;
            }
        }

        private static IUserMessaging _emailSender = null;
        public static IUserMessaging EmailSender
        {
            get
            {
                lock (_lock)
                {
                    if (null == _emailSender) _emailSender = new EmailUserMessaging();
                }
                return _emailSender;
            }
        }

        private static MessageGenerator _messageGen = null;
        public static MessageGenerator MessageGen
        {
            get
            {
                lock (_lock)
                {
                    if (null == _messageGen) _messageGen = new MessageGenerator();
                }
                return _messageGen;
            }
        }

        private static TrackerDirectory _entityUpdateTracker = null;
        public static TrackerDirectory EntityUpdateTracker 
        {
            get
            {
                lock (_lock)
                {
                    if (null == _entityUpdateTracker) _entityUpdateTracker = new TrackerDirectory();
                }
                return _entityUpdateTracker;
            }
        }

        public static void Dispose()
        {
            lock (_lock)
            {
                if (null != _floodPreventor) { _floodPreventor.Dispose(); _floodPreventor = null; }
                if (null != _clientSessionStore) { _clientSessionStore.Dispose(); _clientSessionStore = null; }
                if (null != _fileStorageManager) { _fileStorageManager.Dispose(); _fileStorageManager = null; }
                if (null != _internalFileStorageManager) { _internalFileStorageManager.Dispose(); _internalFileStorageManager = null; }
                if (null != _emailSender) { _emailSender.Dispose(); _emailSender = null; }
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