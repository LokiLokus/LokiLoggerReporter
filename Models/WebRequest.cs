using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lokiloggerreporter.Models
{
    public class WebRequest {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string WebRequestId { get; set; }
        public string HttpMethod { get; set; }
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public string ClientIp { get; set; }
        public string TraceId { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public int StatusCode { get; set; }
        public string Exception { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}