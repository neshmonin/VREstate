using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using Vre.Server.Util;

namespace Vre.Server.Spikes
{
	internal class WmiController
	{
		private long _memoryLimit;
		private Thread _thread;
		private ManualResetEvent _stopEvent;

		public WmiController(long memoryLimit)
		{
			_memoryLimit = memoryLimit;
			_stopEvent = new ManualResetEvent(false);
			_thread = null;
		}

		public void Start() { _thread = new Thread(thread); _stopEvent.Reset(); _thread.Start(); }

		public void Stop() { _stopEvent.Set(); }

		private void thread()
		{
			int errcnt = 0;

			while (!_stopEvent.WaitOne(10000))
			{
				if (errcnt > 60)
				{
					errcnt = 0;

					ServiceInstances.EmailSender.Send(new Messaging.Message(
						Configuration.Messaging.AdminMessageRecipients.Value,
						"WMI Status Error",
						"WMI control is not working; check logs for more details."));
				}

				uint pid;
				try
				{
					pid = ProcessControl.GetPidByServiceName("Winmgmt");
				}
				catch (Exception ex)
				{
					if (errcnt > 60) ServiceInstances.Logger.Error("WMI: Get PID failed 60 times: {0}", ex);
					pid = 0;
				}
				if (0 == pid)
				{
					errcnt++;
					if (errcnt > 60) ServiceInstances.Logger.Error("WMI: Get PID failed 60 times.");
					continue;
				}

				try
				{
					using (var proc = Process.GetProcessById((int)pid))
					{
						long ram = proc.WorkingSet64;
						if (ram > _memoryLimit)
						{
							ServiceInstances.Logger.Warn("WMI: PID={0} using too much memory ({1} MB); attempting restart", pid, ram / (1024*1024));

							bool killit = false, startit = true, error = false;
							try
							{
								if (!tryStopService("Winmgmt", 60000))
								{
									ServiceInstances.Logger.Error("WMI: Cannot stop service - not found!");
									killit = true;
								}
							}
							catch (Exception ex)
							{
								ServiceInstances.Logger.Error("WMI: Stop PID={0} failed: {1}", pid, ex);
								killit = true;
							}
							
							if (killit)
							{
								try
								{
									proc.Kill();
									ServiceInstances.Logger.Info("WMI: Killed PID={0}", pid);
								}
								catch (Exception ex)
								{
									ServiceInstances.Logger.Error("WMI: Kill PID={0} failed: {1}", pid, ex);
									startit = false;
									error = true;
								}
							}

							if (startit)
							{
								try
								{
									if (!startService("Winmgmt", 60000))
									{
										ServiceInstances.Logger.Error("WMI: Cannot start service - not found!");
										error = true;
									}
									else
									{
										ServiceInstances.Logger.Info("WMI: Started.");
									}
								}
								catch (Exception ex)
								{
									ServiceInstances.Logger.Error("WMI: Starting failed: {0}", ex);
									error = true;
								}
							}

							ServiceInstances.EmailSender.Send(new Messaging.Message(
								Configuration.Messaging.AdminMessageRecipients.Value,
								"WMI Status Update",
								error ? "WMI is using too much RAM and is not reacting to control."
								: "WMI was using too much memory and was successfully restarted."));
						}
					}
				}
				catch (Exception ex)
				{
					errcnt++;
					if (errcnt > 60) ServiceInstances.Logger.Error("WMI: Get process failed 60 times: {0}", ex);
				}
			}
		}

		private static bool tryStopService(string svcName, int timeoutMs)
		{
			foreach (var svc in ServiceController.GetServices())
			{
				if (svc.ServiceName.Equals(svcName))
				{
					if (svc.Status == ServiceControllerStatus.Running)
					{
						svc.Stop();
						svc.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 0, 0, timeoutMs));
						return true;
					}
					else
					{
						throw new InvalidOperationException(svcName + " is not in running state");
					}
				}
			}
			return false;
		}

		private static bool startService(string svcName, int timeoutMs)
		{
			foreach (var svc in ServiceController.GetServices())
			{
				if (svc.ServiceName.Equals(svcName))
				{
					if (svc.Status == ServiceControllerStatus.Stopped)
					{
						svc.Start();
						svc.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 0, 0, timeoutMs));
						return true;
					}
					else
					{
						throw new InvalidOperationException(svcName + " is not in stopped state");
					}
				}
			}
			return false;
		}
	}
}