using System;
using System.Collections;
using System.Configuration;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;
using System.Windows.Forms;

namespace Vre.Server
{
    public static class Program
    {
        public static bool Runtime = false;
        public static Configuration Configuration = null;
        public static string ServiceName = "VreServer";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            string cfgFilePath = string.Empty;

            Runtime = true;
            
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            try
            {
                Configuration = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cfgFilePath = Configuration.FilePath;
                string dbVer = Configuration.AppSettings.Settings["ServiceName"].Value;  // file accessibility test
            }
            catch (Exception ex)
            {
                if (Environment.UserInteractive)
                {
                    MessageBox.Show("Cannot access nHconfiguration file (" + cfgFilePath + ").\r\n"
                        + "Make sure server is installed properly.\r\n"
                        + "\r\n" + ex.Message,
                        ServiceName + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    EventLog el = new EventLog("Application", ".", ServiceName);

                    el.WriteEntry(
                        "Cannot access nHconfiguration file (" + cfgFilePath + ").\r\n"
                        + "Make sure server is installed properly.\r\n"
                        + "\r\n" + ex.Message,
                        System.Diagnostics.EventLogEntryType.Error, 1000);
                }
                return 1;
            }
            
            if (Environment.UserInteractive)
            {
                try
                {
                    bool startUI = true;

                    if (args.Length == 1)
                    {
                        if (args[0].ToLower().Equals("install"))
                        {
                            if (!install()) return 2;
                        }
                        else if (args[0].ToLower().Equals("uninstall"))
                        {
                            if (!uninstall()) return 2;
                        }
                        else if (args[0].ToLower().Equals("start"))
                        {
                            new VrService(ServiceName).ServiceControl(VrService.ServiceCommand.Start);
                        }
                        else if (args[0].ToLower().Equals("stop"))
                        {
                            new VrService(ServiceName).ServiceControl(VrService.ServiceCommand.Stop);
                        }
                        else if (args[0].ToLower().Equals("restart"))
                        {
                            new VrService(ServiceName).ServiceControl(VrService.ServiceCommand.Restart);
                        }
                        else if (args[0].ToLower().Equals("debug"))
                        {
                            if (!System.Diagnostics.Debugger.IsAttached)
                                System.Diagnostics.Debugger.Launch();
                        }
                        else
                        {
                            MessageBox.Show("Unknown command line parameter.\r\n",
                                ServiceName + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        startUI = false;
                    }

                    if (startUI)
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new FrmMain());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unhandled error:\r\n"
                        + "\r\n" + ex.Message,
                        ServiceName + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 3;
                }
            }
            else
            {
                try
                {
                    bool startService = true;

                    if (args.Length == 1)
                    {
                        if (args[0].ToLower().Equals("restart"))
                        {
                            new VrService(ServiceName).ServiceControl(VrService.ServiceCommand.Restart);
                        }
                        startService = false;
                    }

                    if (startService)
                    {
                        ServiceBase[] ServicesToRun;

                        // More than one user Service may run within the same service process.
                        // To add another service to this process, change the following line to
                        // create a second service object. For example,
                        //
                        //   ServicesToRun = new System.ServiceProcess.ServiceBase[] {new SampleStarter(), new MySecondUserService()};
                        //
                        ServicesToRun = new ServiceBase[] { new VrService(ServiceName) };
                        ServiceBase.Run(ServicesToRun);
                    }
                }
                catch (Exception ex)
                {
                    EventLog el = new EventLog("Application", ".", ServiceName);

                    el.WriteEntry(
                        "General unhandled error: " + ex.Message,
                        System.Diagnostics.EventLogEntryType.Error, 1010);

                    return 4;
                }
            }

            return 0;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;

            if (Environment.UserInteractive)
            {
                if (ex != null)
                    MessageBox.Show("Unhandled error:\r\n"
                        + "\r\n" + ex.Message,
                        ServiceName + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Unknown unhandled error.",
                        ServiceName + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                EventLog el = new EventLog("Application", ".", ServiceName);
                
                if (ex != null)
                    el.WriteEntry(
                        "General unhandled error: " + ex.Message,
                        System.Diagnostics.EventLogEntryType.Warning, 1011);
                else
                    el.WriteEntry(
                        "General unknown unhandled error.",
                        System.Diagnostics.EventLogEntryType.Warning, 1011);
            }
        }

        private static bool install()
        {
            bool result = true;

            try
            {            
                TransactedInstaller ti = new TransactedInstaller();
                VreServerInstaller mi = new VreServerInstaller();
                ti.Installers.Add(mi);
                String path = String.Format("/assemblypath={0}",
                    System.Reflection.Assembly.GetExecutingAssembly().Location);
                String[] cmdline = { path };
                InstallContext ctx = new InstallContext("", cmdline);
                ti.Context = ctx;
                ti.Install(new Hashtable());
            }
            catch (Exception ex)
            {
                result = false;
                //MessageBox.Show("Installation error.\r\n"
                //    + "\r\n" + ex.Message,
                //    "Vre Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result;
        }

        private static bool uninstall()
        {
            bool result = true;

            try
            {
                TransactedInstaller ti = new TransactedInstaller();
                VreServerInstaller mi = new VreServerInstaller();
                ti.Installers.Add(mi);
                String path = String.Format("/assemblypath={0}",
                    System.Reflection.Assembly.GetExecutingAssembly().Location);
                String[] cmdline = { path };
                InstallContext ctx = new InstallContext("", cmdline);
                ti.Context = ctx;
                ti.Uninstall(null);
            }
            catch (Exception ex)
            {
                result = false;
                //MessageBox.Show("Installation error.\r\n"
                //    + "\r\n" + ex.Message,
                //    "Vre Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result;
        }
    }
}
