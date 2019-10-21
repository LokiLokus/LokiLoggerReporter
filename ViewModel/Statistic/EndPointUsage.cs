using System.Collections.Generic;

namespace lokiloggerreporter.ViewModel.Statistic
{
    public class EndPointUsage
    {
        public List<EndPointUsage> EndPoints { get; set; }
        public string EndPoint { get; set; }
        public int RequestCount { get; set; }
        public int ErrorCount { get; set; }
        public int AverageRequestTime { get; set; }
        public int MinimumRequestTime { get; set; }
        public int MaximumRequestTime { get; set; }
        public int MedianRequestTime { get; set; }
    }
}