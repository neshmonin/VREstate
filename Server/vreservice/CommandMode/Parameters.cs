using System;
using System.Collections.Generic;

namespace Vre.Server.Command
{
    internal class Parameters
    {
        private Dictionary<string, string> _kvp;

        public Parameters(string[] args)
        {
            _kvp = parseOptions(args);
        }

        public string GetOption(string key)
        {
            string result = null;
            _kvp.TryGetValue(key, out result);
            return result;
        }

        public bool ContainsOption(string key)
        {
            string result = null;
            _kvp.TryGetValue(key, out result);
            return (result != null);
        }

        public bool ContainsParameter(string key)
        {
            string result = null;
            _kvp.TryGetValue(key, out result);
            return (null == result);
        }

        public string FilenameByExtension(string extension)
        {
            string result = null;
            if (!extension.StartsWith(".")) extension = "." + extension;
            foreach (string key in _kvp.Keys)
            {
                if (_kvp[key] != null) continue;
                if (key.EndsWith(extension)) { result = key; break; }
            }
            return result;
        }

        private static Dictionary<string, string> parseOptions(string[] elements)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (string e in elements)
            {
                string[] parts = e.Split('=');
                if (parts.Length > 2) throw new ArgumentException("Invalid parameter");
                string key = parts[0];
                string val = (parts.Length == 2) ? parts[1] : null;
                if (result.ContainsKey(key)) throw new ArgumentException("Duplicate parameter");
                result.Add(key, val);
            }

            return result;
        }
    }
}