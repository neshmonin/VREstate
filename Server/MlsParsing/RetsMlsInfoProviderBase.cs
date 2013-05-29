using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace Vre.Server.Mls
{
    public abstract class RetsMlsInfoProviderBase : IMlsInfoProvider
    {
        private string _domain = null;
        private string _uid = null;
        private SecureString _pwd = null;
        private string _executable = null;
        private string _arguments = null;
        private string _resultPath = null;
        private string _resultPattern = null;
        private int _timeoutSec = 0;

        public void Configure(string configurationString)
        {
            foreach (string item in configurationString.Split(';'))
            {
                string[] elements = item.Split('=');
                if (elements.Length != 2) throw new ArgumentException("Configuration item is invalid: " + item);

                string key = elements[0].Trim().ToUpperInvariant();
                string value = elements[1].Trim();

                if (key.Equals("EXECUTABLE")) _executable = value;
                else if (key.Equals("ARGUMENTS")) _arguments = value;
                else if (key.Equals("RESULTPATH")) _resultPath = value;
                else if (key.Equals("RESULTPATTERN")) _resultPattern = value;
                else if (key.Equals("TIMEOUT")) _timeoutSec = int.Parse(value);
                else if (key.Equals("AUTHDOMAIN")) _domain = value;
                else if (key.Equals("AUTHUID")) _uid = value;
                else if (key.Equals("AUTHPWD")) { _pwd = new SecureString(); foreach (char c in value.ToCharArray()) _pwd.AppendChar(c); }
            }
        }

        public void Run()
        {
            if (_executable != null)
            {
                Process proc = new Process();
                proc.StartInfo.FileName = _executable;
                proc.StartInfo.Arguments = _arguments;
                if (_uid != null)
                {
                    proc.StartInfo.Domain = _domain;
                    proc.StartInfo.UserName = _uid;
                    proc.StartInfo.Password = _pwd;
                }

                proc.Start();
                if (!proc.WaitForExit(_timeoutSec * 1000))
                    throw new TimeoutException("Failed running MLS Connector (timeout of " +
                        _timeoutSec.ToString() + "): " + _executable + " " + _arguments);

                if (proc.ExitCode != 0)
                    throw new ApplicationException("Failed running MLS Connector (error " +
                        proc.ExitCode.ToString() + "): " + _executable + " " + _arguments);
            }
		}

		public IList<FileInfo> AvailableFiles
		{
			get
			{
				List<FileInfo> result = new List<FileInfo>();
				foreach (var fileName in Directory.EnumerateFiles(_resultPath, _resultPattern, SearchOption.TopDirectoryOnly))
					result.Add(new FileInfo(fileName));
				return result;
			}
		}

		public string Parse()
		{
			var available = AvailableFiles;

			if (available.Count > 0)
				return Parse(available.OrderBy((a) => a.CreationTimeUtc).Last().FullName);
			else
				return "No file found for parsing.";
		}

		public string Parse(string fileName)
		{
            StringBuilder errors = new StringBuilder();
            List<string> result = new List<string>();

			using (FileStream file = File.OpenRead(fileName))
            {
                using (StreamReader rdr = new StreamReader(file))
                {
                    if (!rdr.EndOfStream) // not empty file
                    {
                        syncUp(CsvUtilities.Split(rdr.ReadLine()));

                        int line = 1;
                        StringBuilder warnings = new StringBuilder();
                        while (!rdr.EndOfStream)
                        {
                            line++;
                            try
                            {
                                process(CsvUtilities.Split(rdr.ReadLine()), warnings);
                            }
                            catch (Exception e)
                            {
                                errors.AppendFormat("Line {0}: {1}", line, e.Message);
                            }
                            if (warnings.Length > 0)
                            {
                                errors.AppendFormat("Line {0}: {1}", line, warnings);
                                warnings.Length = 0;
                            }
                        }
                    }
                }
            }

            if (errors.Length > 0)
                return string.Format("Processing file {0} errors:\r\n{1}", fileName, errors);
            else
                return string.Empty;
        }

        protected abstract void syncUp(string[] header);
        protected abstract void process(string[] row, StringBuilder warnings);

        public abstract IList<string> GetCurrentActiveItems();
        public abstract IList<MlsItem> GetNewItems();
    }
}