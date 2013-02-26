using System;
using System.ServiceProcess;
using System.Threading;

namespace Vre.Server
{
    /// <summary>
    /// Sample service wrapper for Vre Server
    /// </summary>
    public class VrService : ServiceBase
    {
        /// <summary>
        /// Initialize instance properties
        /// </summary>
        public VrService(string serviceName)
        {
            CanPauseAndContinue = false;	// cannot pause this service
            ServiceName = serviceName;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Set things in motion so your service can do its work.
        /// </summary>
        protected override void OnStart(string[] args)
        {
            try
            {
                Thread.CurrentThread.Name = "ServiceMain";
                ServiceRunner.Start();
                base.OnStart(args);
            }
            catch (Exception exOnStart)
            {
                // add logging or other processing if required
                throw exOnStart; // to log in the service's log
            }
        }

        /// <summary>
        /// Stop this service.
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                try
                {
                    ServiceRunner.Stop();
                    base.OnStop();
                }
                catch (Exception)
                {

                }
            }
            catch (Exception)
            {
                // add logging or other processing if required
                throw; // to log in the service's log
            }
        }

        /// <summary>
        /// Executed when the shutdown event is received
        /// </summary>
        protected override void OnShutdown()
        {
            try
            {
                ServiceRunner.Stop();
                base.OnShutdown();
            }
            catch (Exception exOnShutdown)
            {
                // add logging or other processing if required
                throw exOnShutdown; // to log in the service's log
            }
        }

        //public void RestartServer()
        //{
        //    IGenericServices _genericServices = (IGenericServices)Factory.GetInstance(typeof(IGenericServices));

        //    string servicePath = _genericServices.GetMainAssemblyPath();

        //    if (servicePath != null)
        //    {
        //        string serviceName = null;
        //        try
        //        {
        //            serviceName = _genericServices.GetServiceNameByBinaryPath(servicePath);
        //        }
        //        catch (Exception ex)
        //        {
        //            EventLog.WriteEntry(
        //                string.Format("Unable to detect service name by binary path: {0}", ex),
        //                System.Diagnostics.EventLogEntryType.Error, 1001);
        //        }

        //        if (serviceName != null)
        //        {
        //            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(serviceRestarterThread));
        //            t.Start(serviceName);
        //        }
        //        else
        //        {
        //            EventLog.WriteEntry(
        //                "Unable to detect service name by binary path; service restart aborted.",
        //                System.Diagnostics.EventLogEntryType.Warning, 1001);
        //        }
        //    }
        //    else
        //    {
        //        EventLog.WriteEntry(
        //            "Unable to detect service binary path; service restart aborted.",
        //            System.Diagnostics.EventLogEntryType.Error, 1001);
        //    }
        //}

        public enum ServiceCommand { Start, Stop, Restart }

        public void ServiceControl(ServiceCommand command)
        {
            //IGenericServices _genericServices = (IGenericServices)Factory.GetInstance(typeof(IGenericServices));

            //string servicePath = _genericServices.GetMainAssemblyPath();

            //if (servicePath != null)
            {
                string serviceName = null;
                //try
                //{
                //    serviceName = _genericServices.GetServiceNameByBinaryPath(servicePath);
                //}
                //catch (Exception ex)
                //{
                //    EventLog.WriteEntry(
                //        string.Format("Unable to detect service name by binary path: {0}", ex),
                //        System.Diagnostics.EventLogEntryType.Error, 1001);
                //}

                serviceName = Program.ServiceName;

                if (serviceName != null)
                {
                    System.Threading.Thread t = null;
                    switch (command)
                    {
                        case ServiceCommand.Start:
                            t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(serviceStarterThread));
                            break;

                        case ServiceCommand.Stop:
                            t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(serviceStopperThread));
                            break;

                        case ServiceCommand.Restart:
                            t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(serviceRestarterThread));
                            break;
                    }
                    t.Start(serviceName);
                }
                else
                {
                    EventLog.WriteEntry(
                        "Unable to detect service name by binary path; service restart aborted.",
                        System.Diagnostics.EventLogEntryType.Warning, 1001);
                }
            }
            //else
            //{
            //    EventLog.WriteEntry(
            //        "Unable to detect service binary path; service restart aborted.",
            //        System.Diagnostics.EventLogEntryType.Error, 1001);
            //}
        }

        private void serviceStarterThread(object arg)
        {
            string serviceName = arg as string;

            ServiceController sc = new ServiceController(serviceName);

            if (sc.Status == ServiceControllerStatus.Stopped)
            {
                sc.Start();

                sc.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 30));

                if (sc.Status != ServiceControllerStatus.Running)
                {
                    EventLog.WriteEntry(
                        "Timed out waiting for running state.",
                        System.Diagnostics.EventLogEntryType.Warning, 1003);
                }
            }
            else
            {
                EventLog.WriteEntry(
                    "Service is not in stopped state.",
                    System.Diagnostics.EventLogEntryType.Warning, 1003);
            }

            sc.Dispose();
        }

        private void serviceStopperThread(object arg)
        {
            string serviceName = arg as string;

            ServiceController sc = new ServiceController(serviceName);

            if (sc.Status == ServiceControllerStatus.Running)
            {
                sc.Stop();

                sc.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 30));

                if (sc.Status != ServiceControllerStatus.Stopped)
                {
                    EventLog.WriteEntry(
                        "Timed out waiting for stopped state.",
                        System.Diagnostics.EventLogEntryType.Warning, 1004);
                }
            }
            else
            {
                EventLog.WriteEntry(
                    "Service is not in running state.",
                    System.Diagnostics.EventLogEntryType.Warning, 1004);
            }

            sc.Dispose();
        }

        private void serviceRestarterThread(object arg)
        {
            string serviceName = arg as string;

            ServiceController sc = new ServiceController(serviceName);

            sc.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 30));

            if (sc.Status != ServiceControllerStatus.Running)
            {
                EventLog.WriteEntry(
                    "Service is not in running state.  Performing forced shutdown.",
                    System.Diagnostics.EventLogEntryType.Warning, 1005);
            }

            sc.Stop();

            sc.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 1, 0));

            if (sc.Status == ServiceControllerStatus.Stopped)
            {
                sc.Start();
            }
            else
            {
                EventLog.WriteEntry(
                    "Service is not in stopped state (timed out waiting for stop).  Service restart aborted.",
                    System.Diagnostics.EventLogEntryType.Warning, 1005);
            }

            sc.Dispose();
        }
    }
}
