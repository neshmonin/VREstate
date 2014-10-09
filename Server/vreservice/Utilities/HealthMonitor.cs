using System;
using System.Diagnostics;
using System.Threading;

namespace Vre.Server.Util
{
	internal class HealthMonitor
	{
		public const int CpuReadSpanSec = 20;
		public const float CpuUseLimit = 60.0f;

		private long _memoryLimit;
		private Thread _thread;
		private ManualResetEvent _stopEvent;
		private Process _proc;
		private PerformanceCounter _cpu;

		public HealthMonitor(long memoryLimit)
		{
			_memoryLimit = memoryLimit;
			_stopEvent = new ManualResetEvent(false);
			_thread = null;
			_cpu = null;
			_proc = Process.GetCurrentProcess();
		}

		public void Start() { _thread = new Thread(thread) { Priority = ThreadPriority.Highest }; _stopEvent.Reset(); _thread.Start(); }

		public void Stop() { _stopEvent.Set(); }

		private void thread()
		{
			int errcnt = 0;
			Thread.CurrentThread.Name = "HMON#" + Thread.CurrentThread.ManagedThreadId.ToString();

			try
			{
				var procName = HealthInfo.ProcessInstanceName(_proc.Id);

				_cpu = new PerformanceCounter("Process", "% Processor Time", procName, true);
				_cpu.NextValue();
			}
			catch (Exception ex)
			{
				_cpu = null;

				ServiceInstances.Logger.Error("HealthMonitor: failed to initialize a CPU performance counter.", ex);

				ServiceInstances.EmailSender.Send(new Messaging.Message(
					Configuration.Messaging.AdminMessageRecipients.Value,
					"Health Monitor Error",
					"Failed to initialize a CPU performance counter; check logs for more details."));
			}

			while (!_stopEvent.WaitOne(60000))
			{
				if (errcnt > 60)
				{
					errcnt = 0;

					ServiceInstances.EmailSender.Send(new Messaging.Message(
						Configuration.Messaging.AdminMessageRecipients.Value,
						"Health Monitor Error",
						"Monitor is not working; check logs for more details."));
				}

				try
				{
					if (_cpu != null)
					{
						float value = 0.0f;
						for (int idx = CpuReadSpanSec; idx > 0; idx--) { value += _cpu.NextValue(); Thread.Sleep(1000); }
						value /= (float)CpuReadSpanSec;

						if (value > CpuUseLimit)
							ServiceInstances.EmailSender.Send(new Messaging.Message(
								Configuration.Messaging.AdminMessageRecipients.Value,
								"HealthMonitor Status Update",
								string.Format("Service is using too much CPU ({2}% across {0}s; limit is {1}%).", 
									CpuReadSpanSec, CpuUseLimit, value)));
					}

					long ram = _proc.WorkingSet64;
					if (ram > _memoryLimit)
					{
						ServiceInstances.Logger.Warn("HealthMonitor: using too much memory ({0} MB)", ram / (1024 * 1024));

						ServiceInstances.EmailSender.Send(new Messaging.Message(
							Configuration.Messaging.AdminMessageRecipients.Value,
							"HealthMonitor Status Update",
							"Service is using too much RAM."));
					}
				}
				catch (Exception ex)
				{
					errcnt++;
					if (errcnt > 60) ServiceInstances.Logger.Error("HealthMonitor: monitor loop failed 60 times: {0}", ex);
				}
			}
		}
	}
}