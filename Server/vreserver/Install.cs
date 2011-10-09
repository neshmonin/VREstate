using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Vre.Server
{
    /// <summary>
    /// Sample service wrapper installer for Vre Server
    /// </summary>
    [RunInstaller(true)]
    public class VreServerInstaller : Installer
    {

        // Basic service installation
        void ServiceSettings()
        {
            ServiceInstaller sInstaller = new ServiceInstaller();
            ServiceProcessInstaller spInstaller = new ServiceProcessInstaller();

            string serviceName = Program.ServiceName;
            //if (Program.Runtime)
            //{
            //    string name = Program.Configuration.AppSettings.Settings["ServerName"].Value;
            //    if (!string.IsNullOrEmpty(name)) serviceName = name;
            //}
            sInstaller.ServiceName = serviceName;
            sInstaller.DisplayName = serviceName;

            sInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            //sInstaller.ServicesDependedOn = new string[] { "MasterServer" };
            this.Installers.Add(sInstaller);

            spInstaller.Account = ServiceAccount.LocalSystem;
            spInstaller.Username = null;
            spInstaller.Password = null;
            this.Installers.Add(spInstaller);
        }

        void ExceptionSettings()
        {
            //EventLogInstaller exceptionManagerEventLogInstaller = new EventLogInstaller();
            //EventLogInstaller exceptionManagementEventLogInstaller = new EventLogInstaller();

            //exceptionManagerEventLogInstaller.Log = "Application";
            //exceptionManagerEventLogInstaller.Source = Vre.ExceptionManagement.SourceName.Internal;

            //exceptionManagementEventLogInstaller.Log = "Application";
            //exceptionManagementEventLogInstaller.Source = Vre.ExceptionManagement.SourceName.Publish;

            //this.Installers.AddRange(new Installer[] {
            //                                             exceptionManagerEventLogInstaller,
            //                                             exceptionManagementEventLogInstaller});
        }

        #region IDE generated code
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public VreServerInstaller()
        {
            // This call is required by the Designer.
            InitializeComponent();

            ExceptionSettings();
            ServiceSettings();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion
    }
}
