using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;
using Vre.Server.HttpService;
using Vre.Server.RemoteService;

namespace Vre.Server
{
    public class StartupShutdown
    {
        enum ServerRole { VRT, Redirector }

        private static readonly object _lock = new object();
        private static ServerRole _serverRole;
        //private static List<ServiceHost> _hosts = null;
        private static List<string> _listeners = new List<string>();

        public static string Status = "Stopped.";
        public static string[] Listeners { get { lock (_listeners) return _listeners.ToArray(); } }

        private static HttpServiceBase _httpService;

        public static void PerformStartup(bool startAsService)
        {
            ServiceInstances.Logger.Info("Starting server.");
            ServiceInstances.Logger.Info("Version {0}.{1}{2}", 
                Assembly.GetExecutingAssembly().GetName().Version, VersionGen.VersionStamp,
                (VersionGen.IsAlpha) ? 
                    string.Format(" ALPHA {0:yyyyMMddHHmmss}",
                        System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location)) : 
                    string.Empty
            );
            Status = "Starting...";

            try
            {
                if (!Enum.TryParse<ServerRole>(Configuration.Service.ServerRole.Value,
                    true, out _serverRole))
                    _serverRole = ServerRole.VRT;

                switch (_serverRole)
                {
                    case ServerRole.VRT:
                        {
                            _httpService = new HttpServiceMain();

                            DatabaseSettingsDao.VerifyDatabase();

                            RolePermissionCheck.FillAccessMatrix();

                            using (ClientSession vcs = ClientSession.MakeSystemSession())
                            {
                                vcs.Resume();
                                UserManager um = new UserManager(vcs);
                                um.ConfirmPresetUsers();
                            }

                            ServiceInstances.FileCache = new FileCacheManager();

							if (Configuration.ModelCache.Enabled.Value)
							{
								ServiceInstances.ModelCache = new ModelCache.ModelCacheManager(
									Configuration.ModelCache.RootPath.Value);
							}
                        }
                        break;

                    case ServerRole.Redirector:
                        _httpService = new RedirectionService();
                        break;
                }

                if (startAsService)
                {
                    System.Threading.ThreadPool.SetMinThreads(100, 100);

                    if (ServerRole.VRT == _serverRole)
                    {
                        ServiceInstances.FileCache.Initialize();

						if (ServiceInstances.ModelCache != null)
						{
							// This may take long enough for Service Control Manager to render this as hung on start!
							new Thread(() => ServiceInstances.ModelCache.Initialize()) { IsBackground = true, Priority = ThreadPriority.BelowNormal, Name = "InitialModelReader" }
							.Start();
						}
                    }

                    startCommunications();
                }

                Status = "Running.";

                // TODO: DEBUG
                Testing.RandomUpdater.Start(Configuration.Debug.RandomObjectUpdateTimeSec.Value);
            }
            catch (Exception ex)
            {
                ServiceInstances.Logger.Fatal("Startup failed: {0}", Utilities.ExplodeException(ex));
                throw;
            }
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
                //if (null == _hosts)
                //{
                //    _hosts = generateHostList();
                //}

                //lock (_listeners) _listeners.Clear();

                //foreach (ServiceHost host in _hosts)
                //{
                //    try
                //    {
                //        switch (host.State)
                //        {
                //            case CommunicationState.Created:
                //            case CommunicationState.Closed:
                //            case CommunicationState.Faulted:
                //                host.Open();  // TODO: introduce timeout here
                //                break;
                //        }

                //        if (host.State == CommunicationState.Opened)
                //        {
                //            lock (_listeners)
                //            {
                //                foreach (Uri uri in host.BaseAddresses)
                //                {
                //                    _listeners.Add(uri.ToString());
                //                }
                //            }
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        throw new ApplicationException("Failed starting service host [" + host.Description + "].", ex);
                //    }
                //}

                _httpService.PerformStartup();
                _listeners.AddRange(_httpService.Listeners);
            }
        }

        private static void stopCommunications()
        {
            lock (_lock)
            {
                _httpService.PerformShutdown();

                //if (_hosts != null)
                //{
                //    foreach (ServiceHost host in _hosts)
                //    {
                //        try
                //        {
                //            host.Close();  // TODO: introduce timeout here
                //        }
                //        catch (Exception ex)
                //        {
                //            throw new ApplicationException("Failed stopping service host [" + host.Description + "].", ex);
                //        }
                //    }

                //    lock (_listeners) _listeners.Clear();
                //    _hosts = null;
                //}
            }
        }

        //private static List<ServiceHost> generateHostList()
        //{
        //    List<ServiceHost> result = new List<ServiceHost>();

        //    //NetTcpBinding binding = new NetTcpBinding();
        //    //binding.Security.Mode = SecurityMode.Message;
        //    //binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            
        //    //result.Add(new ServiceHost(typeof(UserInterface)));
        //    //result.Add(new ServiceHost(typeof(BuyerInterface)));
        //    //result.Add(new ServiceHost(typeof(SalespersonInterface)));
        //    //result.Add(new ServiceHost(typeof(SubcontractorInterface)));
        //    //result.Add(new ServiceHost(typeof(DeveloperAdminInterface)));

        //    return result;
        //}

		public static void Test()
		{
			var test2 = System.Web.HttpUtility.UrlDecode("");
			var test1 = System.Web.HttpUtility.UrlDecode(null);

			test2.Equals(test1);
		}
    }
}