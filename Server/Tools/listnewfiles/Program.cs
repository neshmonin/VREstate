using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace listnewfiles
{
	class Program
	{
		static int Main(string[] args)
		{
			try
			{
				if (args.Length != 2)
				{
					Console.WriteLine("ListNewFiles v{0}",
						Assembly.GetExecutingAssembly().GetName().Version);

					Console.WriteLine("Usage: {0} <path> <time stamp>",
						Path.GetFileName(Assembly.GetExecutingAssembly().Location));
					Console.WriteLine("All subdirectories are scanned.");
					Console.WriteLine("Time stamp can be in the following formats (tried in this order):");
					Console.WriteLine("- Universal: YYYY-MM-DDTHH:mm:ss.fff{Z|[-]HH:mm}");
					Console.WriteLine("- Universal shortened: YYYY-MM-DDTHH:mm");
					Console.WriteLine("- Clear: YYYYMMDDHHmmss");
					Console.WriteLine("- Clear shortened: YYYYMMDDHHmm");
					Console.WriteLine("- Local format");
					return 1;
				}

				var ts = DateTime.MinValue;
				if (!DateTime.TryParseExact(args[0], new[] {
					"yyyy-MM-ddTHH:mm:ss.fffK",
					"yyyy-MM-ddTHH:mm",
					"yyyyMMddHHmmss",
					"yyyyMMddHHmm"
					}, null, System.Globalization.DateTimeStyles.None, out ts))
				{
					if (!DateTime.TryParse(args[0], out ts))
					{
						Console.WriteLine("ERROR: Time stamp format unknown: \"{0}\"", args[0]);
						return 1;
					}
				}

				if (!Directory.Exists(args[1]))
				{
					Console.WriteLine("ERROR: Path \"{0}\" does not exist.", args[1]);
					return 2;
				}

				var cutLen = args[1].Length + 1;
				foreach (var fn in Directory.EnumerateFiles(args[1], "*.*", SearchOption.AllDirectories))
				{
					var fi = new FileInfo(fn);
					if ((fi.CreationTime > ts) || (fi.LastWriteTime > ts))
						Console.WriteLine(fn.Substring(cutLen));
				}

				return 0;
			}
			catch (Exception ex)
			{
				Console.WriteLine("ERROR: {0}\r\n{1}", ex.Message, ex.StackTrace);
				return 10;
			}
		}
	}
}
