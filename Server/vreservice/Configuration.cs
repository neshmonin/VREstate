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
    }
}