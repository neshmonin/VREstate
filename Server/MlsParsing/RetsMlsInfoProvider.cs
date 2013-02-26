using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using System.IO;
namespace Vre.Server.Mls
{
    public class RetsMlsInfoProvider : IMlsInfoProvider
    {
        private string _domain = null;
        private string _uid = null;
        private SecureString _pwd = null;
        private string _executable = null;
        private string _arguments = null;
        private string _resultPath = null;
        private string _resultPattern = null;
        private int _timeoutSec = 0;
        private int _mlsNumIdx = -1;

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

        public IList<string> GetCurrentActiveItems()
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

            string selectedFile = null;
            DateTime selectedFileCT = DateTime.MinValue;
            foreach (string fileName in Directory.EnumerateFiles(_resultPath, _resultPattern, SearchOption.TopDirectoryOnly))
            {
                FileInfo fi = new FileInfo(fileName);
                if (fi.CreationTimeUtc > selectedFileCT)
                {
                    selectedFileCT = fi.CreationTimeUtc;
                    selectedFile = fileName;
                }
            }

            List<string> result = new List<string>();
            if (selectedFile != null)
            {
                using (FileStream file = File.OpenRead(selectedFile))
                {
                    using (StreamReader rdr = new StreamReader(file))
                    {
                        if (!rdr.EndOfStream) // not empty file
                        {
                            syncUp(CsvUtilities.Split(rdr.ReadLine()));

                            while (!rdr.EndOfStream) result.Add(process(CsvUtilities.Split(rdr.ReadLine())));
                        }
                    }
                }
            }

            return result;
        }

        private void syncUp(string[] header)
        {
            for (int idx = header.Length - 1; idx >= 0; idx--)
            {
                if (header[idx].Equals("Ml_num")) _mlsNumIdx = idx;
            }

            if (_mlsNumIdx < 0)
                throw new ApplicationException("Sync file has no header or header is invalid");
        }

        private string process(string[] row)
        {
            if (row.Length > _mlsNumIdx) return row[_mlsNumIdx];
            else throw new ApplicationException("Sync file has invalid content");
        }
    }
}