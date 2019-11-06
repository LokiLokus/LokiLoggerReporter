using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using lokiloggerreporter.Models;
using Newtonsoft.Json;

namespace lokiloggerreporter.ViewModel.Statistic
{
    public class EndPointUsage
    {
        [JsonIgnore]
        public EndPointUsage Parent { get; set; }
        
        public List<EndPointUsage> EndPoints { get; set; } = new List<EndPointUsage>();
        public string EndPoint { get; set; }
        [JsonIgnore]
        public List<WebRequest> WebRequests { get; set; } = new List<WebRequest>();
        public int RequestCount { get; set; }
        public int ErrorCount { get; set; }
        public int AverageRequestTime { get; set; }
        public int MinimumRequestTime { get; set; }
        public int MaximumRequestTime { get; set; }
        public int MedianRequestTime { get; set; }
[JsonIgnore]
        public List<RequestAnalyzeModel> _concurrentRequests { get; set; } = new List<RequestAnalyzeModel>();
        public List<RequestAnalyzeModel> Requests { get
            {
                return _concurrentRequests.ToList().OrderBy(x => x.FromTime).ToList();
            }}
        [JsonIgnore] public bool Processed { get; set; }
        public long AbsoluteRequestTime { get; set; }
    }

    public class RequestAnalyzeModel
    {
        public DateTime ToTime { get; set; }
        public DateTime FromTime { get; set; }

        public long UnixTime
        {
            get
            {
                return (long)FromTime
                    .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                    .TotalMilliseconds;
            }
        }

        public int ErrorCount { get; set; }
        public int RequestCount { get; set; }
        
        public List<WebRequest> InterestingRequestModel { get; set; } = new List<WebRequest>();
    }
}