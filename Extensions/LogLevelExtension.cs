using System.Collections.Generic;
using lokiloggerreporter.Models;

namespace lokiloggerreporter.Extensions {
	public static class LogLevelExtension {

		public static List<LogLevel> Levels()
		{
			return new List<LogLevel>()
			{
				LogLevel.Verbose,
				LogLevel.Debug,
				LogLevel.Information,
				LogLevel.Warning,
				LogLevel.Critical,
				LogLevel.SystemCritical
			};
		}
	}
}