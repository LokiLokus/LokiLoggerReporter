using System.Linq;
using System.Threading.Tasks;
using lokiloggerreporter.Hubs;
using lokiloggerreporter.Models;
using Microsoft.EntityFrameworkCore;

namespace lokiloggerreporter.Services.Implementation
{
    public class LogsObtainerService
    {
        public DatabaseCtx DatabaseCtx { get; set; }

        public LogsObtainerService(DatabaseCtx db)
        {
            DatabaseCtx = db;
        }

        public async Task<ReturnData> ObtainLogs(RequestModel model)
        {
            ReturnData result = new ReturnData()
            {
                Count = model.Count,
                From = model.From,
            };
            if (string.IsNullOrWhiteSpace(model.ExcludeRest)) model.ExcludeRest = null;
            if (string.IsNullOrWhiteSpace(model.IncludeRest)) model.IncludeRest = null;
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
            ).Include(x => x.WebRequest);
            result.TotalCount = await query.CountAsync();
            result.Logs = await query.Skip(model.From).Take(model.Count).Include(x => x.WebRequest).ToListAsync();
            
            
            return result;
        }
    }
}