using System;
using System.Configuration;

namespace Vre.Server
{
    public class Config
    {
        private Configuration _configuration;

        public Config()
        {
            _configuration = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        //public Configuration Configuration { get { return _configuration; } }

        public string FilePath { get { return _configuration.FilePath; } }

        public string GetValue(string key, string defaultValue)
        {
            string result = defaultValue;
            try
            {
                KeyValueConfigurationElement kv = _configuration.AppSettings.Settings[key];
                if (kv != null) result = kv.Value;
            }
            catch { }
            return result;
        }

        public int GetValue(string key, int defaultValue)
        {
            string val = GetValue(key, defaultValue.ToString());
            int ival;
            if (int.TryParse(val, out ival)) return ival;
            return defaultValue;
        }

        public bool GetValue(string key, bool defaultValue)
        {
            string val = GetValue(key, defaultValue.ToString())
                .ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim();
            bool result = defaultValue;

            if (val.Equals("true") || val.Equals("t") || val.Equals("yes") || val.Equals("y") || val.Equals("1"))
                result = true;

            if (val.Equals("false") || val.Equals("f") || val.Equals("no") || val.Equals("n") || val.Equals("0"))
                result = false;

            return result;
        }
    }
}