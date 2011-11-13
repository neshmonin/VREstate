using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace decout
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Output decorator, v{0}", Assembly.GetExecutingAssembly().GetName().Version);
                Console.WriteLine("Usage: DECOUT <template file> <app to execute> [<params> ...]");
                Console.WriteLine("Template file contents must conform to .NET's string.Format() format parameter");
                Console.WriteLine("and contain a single parameter placeholder ({0}).");
                return 1;
            }

            try
            {
                string template;
                using (FileStream fs = File.OpenRead(args[0]))
                {
                    using (StreamReader sr = new StreamReader(fs)) template = sr.ReadToEnd();
                }

                StringBuilder argsCombined = new StringBuilder();
                for (int idx = 2; idx < args.Length; idx++) { argsCombined.Append(args[idx]); argsCombined.Append(" "); }

                ProcessStartInfo psi = new ProcessStartInfo(args[1], argsCombined.ToString());
                psi.RedirectStandardOutput = true;
                psi.UseShellExecute = false;
                Process proc = Process.Start(psi);
                string output = proc.StandardOutput.ReadToEnd();

                Console.Write(string.Format(template, output.Trim()));

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ERROR: {0}", ex.Message);
                return 2;
            }
        }
    }
}
