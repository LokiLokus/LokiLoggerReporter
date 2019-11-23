using System;
using System.Collections.Generic;

namespace lokiloggerreporter.Services {
	public class SettingService :ISettingsService {
		private Dictionary<string, object> dataStorage = new Dictionary<string, object>();
		
		
		
		public T Get<T>(string key)
		{
			if (dataStorage.ContainsKey(key))
			{
				try
				{
					return (T)dataStorage[key];
				}
				catch (Exception e)
				{
					return default(T);
				}
			}
			return default(T);
		}

		public void Set<T>(string key, T data)
		{
			dataStorage[key] = data;
		}

		public T Get<T>(string key, T defaultVal)
		{
			if (dataStorage.ContainsKey(key))
			{
				try
				{
					return (T)dataStorage[key];
				}
				catch (Exception e)
				{
					return defaultVal;
				}
			}
			return defaultVal;
		}
	}
}