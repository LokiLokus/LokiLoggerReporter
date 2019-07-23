namespace lokiloggerreporter.Services {
	public interface ISettingsService {
		T Get<T>(string key);
		void Set<T>(string key, T data);
		T Get<T>(string key, T defaultVal);
	}
}