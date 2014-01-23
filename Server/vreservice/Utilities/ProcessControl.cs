using System;
using System.Management;

namespace Vre.Server.Util
{
	internal class ProcessControl
	{
		/// <summary>
		/// Returns a PID of a running service or zero if service does not run.
		/// <para>Note that PID may be shared by multiple services.</para>
		/// </summary>
		public static uint GetPidByServiceName(string svcName)
		{
			uint result = 0;
			using (var srch = new ManagementObjectSearcher(string.Format("SELECT ProcessId FROM Win32_Service WHERE Name='{0}'", svcName)))
			{
				foreach (var obj in srch.Get())
					if (result != 0) throw new InvalidOperationException("Multiple PIDs exist for same service");
					else result = (uint)obj["ProcessId"];
			}
			return result;
		}
	}
}