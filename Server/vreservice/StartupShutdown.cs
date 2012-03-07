using System;
using System.Collections.Generic;
using System.ServiceModel;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;
using Vre.Server.HttpService;
using Vre.Server.RemoteService;
using System.Threading;

namespace Vre.Server
{
    public class StartupShutdown
    {
        private static readonly object _lock = new object();
        private static List<ServiceHost> _hosts = null;
        private static List<string> _listeners = new List<string>();

        public static string Status = "Stopped.";
        public static string[] Listeners { get { lock (_listeners) return _listeners.ToArray(); } }

        public static void PerformStartup(bool startAsService)
        {
            ServiceInstances.Logger.Info("Starting server.");
            Status = "Starting...";

            DatabaseSettingsDao.VerifyDatabase();

            using (ClientSession vcs = ClientSession.MakeSystemSession())
            {
                vcs.Resume();
                UserManager um = new UserManager(vcs);
                um.ConfirmPresetUsers();
            }

            ServiceInstances.FileCache = new FileCacheManager();

            ServiceInstances.ModelCache = new ModelCache.ModelCacheManager(
                ServiceInstances.Configuration.GetValue("ModelFileStore", string.Empty));

            if (startAsService)
            {
                System.Threading.ThreadPool.SetMinThreads(100, 100);

                // This may take long enough for Service Control Manager to render this as hung on start!
                new Thread(() => ServiceInstances.FileCache.Initialize()) 
                { IsBackground = true, Priority = ThreadPriority.BelowNormal, Name = "InitializeFileCache" }
                .Start();

                // This may take long enough for Service Control Manager to render this as hung on start!
                new Thread(() => ServiceInstances.ModelCache.Initialize()) 
                { IsBackground = true, Priority = ThreadPriority.BelowNormal, Name = "InitialModelReader" }
                .Start();
                
                startCommunications();
            }

            Status = "Running.";

#if DEBUG
            Testing.RandomUpdater.Start();
#endif
        }

        public static void PerformShutdown()
        {
            ServiceInstances.Logger.Info("Stopping server.");
            Status = "Stopping...";
            
            stopCommunications();

            ServiceInstances.Dispose();

            Status = "Stopped.";
            ServiceInstances.Logger.Info("Server stopped.");
        }

        private static void startCommunications()
        {
            lock (_lock)
            {
                if (null == _hosts)
                {
                    _hosts = generateHostList();
                }

                lock (_listeners) _listeners.Clear();

                foreach (ServiceHost host in _hosts)
                {
                    try
                    {
                        switch (host.State)
                        {
                            case CommunicationState.Created:
                            case CommunicationState.Closed:
                            case CommunicationState.Faulted:
                                host.Open();  // TODO: introduce timeout here
                                break;
                        }

                        if (host.State == CommunicationState.Opened)
                        {
                            lock (_listeners)
                            {
                                foreach (Uri uri in host.BaseAddresses)
                                {
                                    _listeners.Add(uri.ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Failed starting service host [" + host.Description + "].", ex);
                    }
                }

                HttpServiceManager.PerformStartup();
                _listeners.AddRange(HttpServiceManager.Listeners);
            }
        }

        private static void stopCommunications()
        {
            lock (_lock)
            {
                HttpServiceManager.PerformShutdown();

                if (_hosts != null)
                {
                    foreach (ServiceHost host in _hosts)
                    {
                        try
                        {
                            host.Close();  // TODO: introduce timeout here
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException("Failed stopping service host [" + host.Description + "].", ex);
                        }
                    }

                    lock (_listeners) _listeners.Clear();
                    _hosts = null;
                }
            }
        }

        private static List<ServiceHost> generateHostList()
        {
            List<ServiceHost> result = new List<ServiceHost>();

            //NetTcpBinding binding = new NetTcpBinding();
            //binding.Security.Mode = SecurityMode.Message;
            //binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            
            //result.Add(new ServiceHost(typeof(UserInterface)));
            //result.Add(new ServiceHost(typeof(BuyerInterface)));
            //result.Add(new ServiceHost(typeof(SalespersonInterface)));
            //result.Add(new ServiceHost(typeof(SubcontractorInterface)));
            //result.Add(new ServiceHost(typeof(DeveloperAdminInterface)));

            return result;
        }
    }
}