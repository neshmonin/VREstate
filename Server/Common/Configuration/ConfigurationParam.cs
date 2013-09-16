using System;

namespace Vre
{
	public abstract class ConfigurationParam<T>
	{
		protected readonly ConfigurationBase _config;
		protected readonly string _key;
		protected readonly T _default;
		private T _value;
		private bool _isActual;

		public ConfigurationParam(ConfigurationBase config, string key, T defaultValue)
		{
			_key = key;
			_default = defaultValue;
			_isActual = false;
			_config = config;
			_config.OnModified += new EventHandler((s, e) =>
			{
				_isActual = false;
				if (OnModified != null) OnModified(e, e);
			});
		}

		public EventHandler OnModified;

		public T Value
		{
			get
			{
				if (!_isActual)
				{
					_value = getValue();
					_isActual = true;
				}
				return _value;
			}
		}

		protected abstract T getValue();
	}

	public class BooleanConfigurationParam : ConfigurationParam<bool>
	{
		public BooleanConfigurationParam(ConfigurationBase config, string key, bool defaultValue)
			: base(config, key, defaultValue) { }
		protected override bool getValue()
		{
			return _config.GetValue(_key, _default);
		}
	}

	public class IntegerConfigurationParam : ConfigurationParam<int>
	{
		private int _min, _max;

		public IntegerConfigurationParam(ConfigurationBase config, string key, int defaultValue)
			: base(config, key, defaultValue) 
		{
			_min = int.MinValue;
			_max = int.MaxValue;
		}
		
		public IntegerConfigurationParam(ConfigurationBase config, string key, 
			int defaultValue, int minimalValue, int maximalValue)
			: base(config, key, defaultValue) 
		{
			if (_max < _min) throw new ArgumentException("Maximal must be greater or equal to minimal.");
			_min = minimalValue;
			_max = maximalValue;
		}

		protected override int getValue()
		{
			var result = _config.GetValue(_key, _default);
			if (result < _min) result = _min;
			else if (result > _max) result = _max;
			return result;
		}
	}

	public class StringConfigurationParam : ConfigurationParam<string>
	{
		public StringConfigurationParam(ConfigurationBase config, string key, string defaultValue)
			: base(config, key, defaultValue) { }
		protected override string getValue()
		{
			return _config.GetValue(_key, _default);
		}
	}
}