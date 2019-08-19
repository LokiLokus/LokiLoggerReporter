using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using lokiloggerreporter.Models;

namespace lokiloggerreporter.ViewModel
{
    public class SendedLogModel
    {
        public string SourceId { get; set; }
        public string SourceSecret { get; set; }
        public List<SendedLogs> Logs { get; set; }
        
    }

    public class SendedLogs
    {
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
    }
}