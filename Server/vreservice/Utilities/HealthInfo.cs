using System;
using System.Diagnostics;
using System.Management;
using System.Text;
using System.Threading;

namespace Vre.Server.Util
{
	internal class HealthInfo
	{
		class ProcessInfo
		{
			public string ServiceName = null;
			public string ProcName = null;
			public uint Pid = 0;
			public PerformanceCounter CpuUsage = null;
		}

		public const double ReportLifetimeSec = 10.0;

		private static readonly object _reportLock = new object();
		private static DateTime _lastBuild = DateTime.MinValue;
		private static string _lastReport = null;

		private static PerformanceCounter _cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
		private static PerformanceCounter _ramCounter = new PerformanceCounter("Memory", "Available MBytes", true);
		private static ProcessInfo _this = new ProcessInfo() { Pid = (uint)Process.GetCurrentProcess().Id };
		private static ProcessInfo _dbms = new ProcessInfo() { ServiceName = "MSSQLSERVER" };
		private static ProcessInfo _wmi = new ProcessInfo() { ServiceName = "Winmgmt" };

		public static string BuildHealthReport()
		{
			var now = DateTime.UtcNow;
			string result;

			lock (_reportLock)
			{
				if ((now.Subtract(_lastBuild).TotalSeconds > ReportLifetimeSec) || (null == _lastReport))
				{
					_lastBuild = now;
					_lastReport = buildHealthReport();
				}
				result = _lastReport;
			}

			return result;
		}

		private static string buildHealthReport()
		{
			var now = DateTime.Now;
			var result = new StringBuilder();

			result.Append("System stats:");

			result.AppendFormat("\r\nCPU load: {0}%\r\n", _cpu.NextValue());

			result.AppendFormat("Free RAM: {0} MbB\r\n", _ramCounter.NextValue());

			result.Append("\r\nCurrent process:\r\n");
			dumpProcessInfo(now, result, _this);

			result.Append("\r\nDBMS:\r\n");
			dumpProcessInfo(now, result, _dbms);

			result.Append("\r\nWMI:\r\n");
			dumpProcessInfo(now, result, _wmi);

			return result.ToString();
		}

		private static void dumpProcessInfo(DateTime now, StringBuilder result, ProcessInfo info)
		{
			uint pid = info.Pid;
			if (info.ServiceName != null)  // external service: get PID for verification
			{
				try
				{
					pid = ProcessControl.GetPidByServiceName(info.ServiceName);
				}
				catch
				{
					pid = 0;
				}
			}
			if (pid != 0)
			{
				try
				{
					if (pid != info.Pid)  // external process restarted
					{
						info.Pid = pid;
						info.ProcName = null;
					}

					if (null == info.ProcName)
					{
						info.ProcName = ProcessInstanceName((int)pid);

						if (info.CpuUsage != null) info.CpuUsage.Dispose();
						info.CpuUsage = null;
					}

					if (null == info.CpuUsage)
					{
						info.CpuUsage = new PerformanceCounter("Process", "% Processor Time", info.ProcName, true);
						info.CpuUsage.NextValue(); Thread.Sleep(1000);
					}

					dumpProcessStats(now, result, info);
				}
				catch 
				{
					result.Append("  <<< ERROR-1 >>>\r\n");
				}
			}
			else
			{
				result.Append("  N/A\r\n");
			}
		}

		public static string ProcessInstanceName(int pid)
		{
			var cat = new PerformanceCounterCategory("Process");

			foreach (var inst in cat.GetInstanceNames())
			{
				using (var cnt = new PerformanceCounter("Process", "ID Process", inst, true))
					if (cnt.RawValue == pid) return inst;
			}

			return null;
		}

		private static void dumpProcessStats(DateTime now, StringBuilder result, ProcessInfo info)
		{
			const long mbb = 1024 * 1024;

			using (var cp = Process.GetProcessById((int)info.Pid))
			{
				result.AppendFormat("  Uptime: {0:d\\ hh\\:mm\\:ss}\r\n",
					now.Subtract(cp.StartTime));

				result.AppendFormat("  CPU use: {0}%\r\n", info.CpuUsage.NextValue());

				result.AppendFormat("  Process memory (MbB):\r\n");
				result.AppendFormat("  Working Set: {0}/{1}\r\n",
					cp.WorkingSet64 / mbb, cp.PeakWorkingSet64 / mbb);
				result.AppendFormat("  Non-paged: {0}\r\n",
					cp.NonpagedSystemMemorySize64 / mbb);
				result.AppendFormat("  Paged: {0}/{1}\r\n",
					cp.PagedMemorySize64 / mbb, cp.PeakPagedMemorySize64 / mbb);
				result.AppendFormat("  Paged System: {0}\r\n",
					cp.PagedSystemMemorySize64 / mbb);
				result.AppendFormat("  Virtual: {0}/{1}\r\n",
					cp.VirtualMemorySize64 / mbb, cp.PeakVirtualMemorySize64 / mbb);
			}
		}
	}
}