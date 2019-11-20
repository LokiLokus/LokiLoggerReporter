using System;
using System.ComponentModel.DataAnnotations;

namespace lokiloggerreporter.ViewModel.Statistic
{
    public class RestAnalyzeRequestModel
    {
        [Required]
        public string SourceId { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public bool Ignore404 { get; set; }
        public uint Resolution { get; set; }
    }
}