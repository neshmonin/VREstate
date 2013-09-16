using System;

namespace Vre
{
	public abstract class ConfigurationBase
	{
		public EventHandler OnModified;

		public abstract bool GetValue(string key, bool defaultValue);
		public abstract int GetValue(string key, int defaultValue);
		public abstract string GetValue(string key, string defaultValue);
	}
}
