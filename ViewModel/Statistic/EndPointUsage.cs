using System.Collections.Generic;
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
        
        [JsonIgnore] public bool Processed { get; set; }
    }
}