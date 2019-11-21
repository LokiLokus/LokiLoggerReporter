using System.Collections.Generic;
using lokiloggerreporter.Models;

namespace lokiloggerreporter.Extensions {
	public static class LogTypExtension {
		public static List<LogTyp> Typs => new List<LogTyp>()
		{
			LogTyp.Exception,
			LogTyp.Invoke,
			LogTyp.Normal,
			LogTyp.Return
		};
	}
}