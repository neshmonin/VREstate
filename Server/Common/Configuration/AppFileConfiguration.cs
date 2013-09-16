using System;
using System.IO;

namespace Vre
{
	public class AppFileConfiguration : ConfigurationBase
	{
		private System.Configuration.Configuration _configuration;
		private FileSystemWatcher _watcher;

		public AppFileConfiguration()
		{
			_configuration = System.Configuration.ConfigurationManager.OpenExeConfiguration(
				System.Configuration.ConfigurationUserLevel.None);

			_watcher = new FileSystemWatcher(
				Path.GetDirectoryName(_configuration.FilePath),
				Path.GetFileName(_configuration.FilePath));
			_watcher.Changed += _watcher_Changed;
			_watcher.EnableRaisingEvents = true;
		}

		private void _watcher_Changed(object sender, FileSystemEventArgs e)
		{
			// TODO: Fix multiple events!
			if (OnModified != null) OnModified(sender, EventArgs.Empty);
		}

		//public Configuration Configuration { get { return _configuration; } }

		//public string FilePath { get { return _configuration.FilePath; } }

		public override string GetValue(string key, string defaultValue)
		{
			string result = defaultValue;
			try
			{
				System.Configuration.KeyValueConfigurationElement kv = _configuration.AppSettings.Settings[key];
				if (kv != null) result = kv.Value;
			}
			catch { }
			return result;
		}

		public override int GetValue(string key, int defaultValue)
		{
			string val = GetValue(key, defaultValue.ToString());
			int ival;
			if (int.TryParse(val, out ival)) return ival;
			return defaultValue;
		}

		public override bool GetValue(string key, bool defaultValue)
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