using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using lokiloggerreporter.Models;
using Microsoft.AspNetCore.SignalR;

namespace lokiloggerreporter.Hubs
{
    public class AnalyzeHub : Hub        
    {
        public async Task Request(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }

    class RequestModel
    {
        public bool Debug { get; set; }
        public bool Info { get; set; }
        public bool Warn { get; set; }
        public bool Error { get; set; }
        public bool Critical { get; set; }

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        
        public bool Auto { get; set; }
        public bool Normal { get; set; }
        public bool Invoke { get; set; }
        public bool Return { get; set; }
        public bool Exec { get; set; }
        public bool Rest { get; set; }

        public string ExcludeAll { get; set; }
        public string IncludeAll { get; set; }

        public string IncludeData { get; set; }
        public string ExcludeData { get; set; }
        
        public string IncludeException { get; set; }
        public string ExcludeException { get; set; }

        
        public string SourceId { get; set; }
        
        public string IncludeClass { get; set; }
        public string ExcludeClass { get; set; }

        public string IncludePath { get; set; }
        public string ExcludePath { get; set; }

        public int? ThreadId { get; set; }
        
    }

    class ReturnData
    {
        public List<Log> Logs { get; set; }
        public int From { get; set; }
        public int Count { get; set; }
        public int TotalCount { get; set; }
    }
}