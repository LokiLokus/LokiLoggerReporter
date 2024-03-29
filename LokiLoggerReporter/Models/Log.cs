using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lokiloggerreporter.Models {
	public class Log {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		public int ThreadId { get; set; }
		public DateTime Time { get; set; }
		public LogLevel LogLevel { get; set; }
		public string Message { get; set; }
		public string Class { get; set; }
		public string Method { get; set; }
		public int Line { get; set; }
		public LogTyp LogTyp { get; set; }
		public string Exception { get; set; }
		public string Data { get; set; }

		[ForeignKey("Source")]
		public string SourceId { get; set; }
		public Source Source { get; set; }
		
		[ForeignKey("WebRequest")]
		public string WebRequestId { get; set; }

		public WebRequest WebRequest { get; set; }

	}
	public enum LogTyp
	{
		Normal,
		Exception,
		Return,
		Invoke,
		RestCall
	}
	public enum LogLevel {
		Verbose,
		Debug,
		Information,
		Warning,
		Critical,
		SystemCritical
	}
	
}