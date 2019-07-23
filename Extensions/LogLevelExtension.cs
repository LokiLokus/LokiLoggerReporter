using System.Collections.Generic;
using lokiloggerreporter.Models;

namespace lokiloggerreporter.Extensions {
	public static class LogLevelExtension {

		public static List<LogLevel> Levels()
		{
			return new List<LogLevel>()
			{
				LogLevel.Critical,
				LogLevel.Debug,
				LogLevel.Information,
				LogLevel.Verbose,
				LogLevel.Warning,
				LogLevel.SystemCritical
			};
		}
	}
}