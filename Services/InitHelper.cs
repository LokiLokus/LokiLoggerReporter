using lokiloggerreporter.Models;

namespace lokiloggerreporter.Services {
	public static class InitHelper {

		public static void AddLogs(DatabaseCtx ctx)
		{
			ctx.Logs.Add(new Log()
			{
				Name = "Test",
				Class = "asdas",
				Data = "asdpkdf",
				LogTyp = LogTyp.Exception,
				LogLevel = LogLevel.Critical
			});
			ctx.Logs.Add(new Log()
			{
				Name = "Test",
				Class = "sdfgsdfg",
				Data = "Information",
				LogTyp = LogTyp.Normal,
				LogLevel = LogLevel.Information
			});
			ctx.Logs.Add(new Log()
			{
				Name = "RRS",
				Class = "sdfgsdfg",
				Data = "Information",
				LogTyp = LogTyp.Normal,
				LogLevel = LogLevel.Information
			});
			ctx.SaveChanges();
		}
	}
}