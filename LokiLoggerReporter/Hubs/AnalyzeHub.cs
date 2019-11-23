using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lokiloggerreporter.Models;
using lokiloggerreporter.Services.Implementation;
using lokiloggerreporter.ViewModel.Statistic;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace lokiloggerreporter.Hubs
{
    public class AnalyzeHub : Hub
    {
        public RestAnalyzeService RestAnalyzeService { get; set; }
        public AnalyzeHub(DatabaseCtx db,RestAnalyzeService service)
        {
            RestAnalyzeService = service;
            DatabaseCtx = db;
        }

        public DatabaseCtx DatabaseCtx { get; set; }

        public async Task<EndPointUsage> RequestAnalyseUsage(RestAnalyzeRequestModel model)
        {

            return await RestAnalyzeService.GetEndPointUsageStatistic(model);
        }
        

        public async Task<ReturnData> Request(RequestModel model)
        {
            ReturnData result = new ReturnData()
            {
                Count = model.Count,
                From = model.From,
            };
            if (string.IsNullOrWhiteSpace(model.ExcludeRest)) model.ExcludeRest = null;
            if (string.IsNullOrWhiteSpace(model.IncludeRest)) model.IncludeRest = null;
            /*
            IQueryable<Log> query = DatabaseCtx.Logs.Where(x =>
                x.SourceId == model.SourceId &&
                (model.Debug && x.LogLevel == LogLevel.Debug ||
                 model.Info && x.LogLevel == LogLevel.Information ||
                 model.Warn && x.LogLevel == LogLevel.Warning ||
                 model.Error && x.LogLevel == LogLevel.Critical ||
                 model.Critical && x.LogLevel == LogLevel.SystemCritical) &&
                (model.Invoke && x.LogTyp == LogTyp.Invoke ||
                 model.Normal && x.LogTyp == LogTyp.Normal ||
                 model.Return && x.LogTyp == LogTyp.Return ||
                 model.Exec && x.LogTyp == LogTyp.Exception ||
                 model.Exec && x.LogTyp == LogTyp.Exception ||
                 model.Rest && x.LogTyp == LogTyp.RestCall) &&
                (model.ThreadId == null || x.ThreadId == model.ThreadId) &&
                (model.FromTime == null || x.Time >= model.FromTime) &&
                (model.ToTime == null || x.Time <= model.ToTime) &&
                (model.IncludeRest == null || x.WebRequest.Path.Contains(model.IncludeRest)) &&
                (model.ExcludeRest == null || !x.WebRequest.Path.Contains(model.ExcludeRest))
            );
            result.TotalCount = await query.CountAsync();
            result.Logs = await query.Skip(model.From).Take(model.Count).Include(x => x.WebRequest).ToListAsync();
            
            */
            return result;
        }
    }

    public class WebRequestModel
    {
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }

        public int From { get; set; }
        public int Count { get; set; }
        public string IncludePath { get; set; }
        public string ClientIp { get; set; }
        public bool StatusCode100 { get; set; }
        public bool StatusCode200 { get; set; }
        public bool StatusCode300 { get; set; }
        public bool StatusCode400 { get; set; }
        public bool StatusCode500 { get; set; }
        public bool StatusCode900 { get; set; }
        public bool Ignore404 { get; set; } = true;
        public bool? Post { get; set; }
        public bool? Get { get; set; }
        public bool? Put { get; set; }
        public bool? Delete { get; set; }
        public bool? Patch { get; set; }
        public bool? Options { get; set; }

        public string SourceId { get; set; }
    }
    
    public class RequestModel
    {
        public bool? Debug { get; set; }
        public bool? Info { get; set; }
        public bool? Warn { get; set; }
        public bool? Error { get; set; }
        public bool? Critical { get; set; }

        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }

        public int From { get; set; }
        public int Count { get; set; }

        public bool? Normal { get; set; }
        public bool? Invoke { get; set; }
        public bool? Return { get; set; }
        public bool? Exec { get; set; }
        public bool? Rest { get; set; }

        public string IncludeRest { get; set; }
        public string ExcludeRest { get; set; }

        public string SourceId { get; set; }
        public int? ThreadId { get; set; }
        public bool? SystemGenerated { get; set; }
    }

    public class ReturnData
    {
        public List<Log> Logs { get; set; }
        public int From { get; set; }
        public int Count { get; set; }
        public int TotalCount { get; set; }
    }
    
    public class WebReturnData
    {
        public List<WebRequest> Logs { get; set; }
        public int From { get; set; }
        public int Count { get; set; }
        public int TotalCount { get; set; }
    }
}