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

        public bool RemoveKey(string key)
        {
            bool result = false;
            if (_kvp.ContainsKey(key))
            {
                _kvp.Remove(key);
                result = true;
            }
            return result;
        }

        public string GetOption(string key)
        {
            string result = null;
            if (!_kvp.TryGetValue(key, out result)) return null;
            return result;
        }

        public bool ContainsOption(string key)
        {
            string result;
            if (!_kvp.TryGetValue(key, out result)) return false;
            return (result != null);
        }

        public bool ContainsParameter(string key)
        {
            string result;
            if (!_kvp.TryGetValue(key, out result)) return false;
            return (null == result);
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