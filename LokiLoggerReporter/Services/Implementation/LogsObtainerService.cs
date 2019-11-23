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

        public async Task<WebReturnData> ObtainWebRequests(WebRequestModel model)
        {
            WebReturnData result = new WebReturnData()
            {
                Count = model.Count,
                From = model.From,
            };
            var logs = await DatabaseCtx.WebRequest.Where(x => DatabaseCtx.Logs.Where(z => z.SourceId == model.SourceId && 
                                                                                           (model.FromTime == null || x.Start >= model.FromTime) &&
                                                                                           (model.ToTime == null || x.Start <= model.ToTime)&&
                                                                                           (model.Ignore404 == false || x.StatusCode != 404)&&
                                                                                           z.LogTyp == LogTyp.RestCall
            ).Any(z => z.WebRequestId == x.WebRequestId))
                .Where(x => (!model.Ignore404 || x.StatusCode != 404) &&
                            (model.ClientIp == null || x.ClientIp.Contains(model.ClientIp))&&
                            (model.IncludePath == null || (x.Scheme + x.Host + x.Path + x.QueryString).Contains(model.IncludePath)) &&
                            ((model.StatusCode100 && x.StatusCode >= 0 && x.StatusCode <= 199))||
                            ((model.StatusCode200 && x.StatusCode >= 200 && x.StatusCode <= 299))||
                            ((model.StatusCode300 && x.StatusCode >= 300 && x.StatusCode <= 399))||
                            ((model.StatusCode400 && x.StatusCode >= 400 && x.StatusCode <= 499))||
                            ((model.StatusCode500 && x.StatusCode >= 500 && x.StatusCode <= 599))||
                            ((model.StatusCode900 && x.StatusCode >= 600))).OrderBy(x => x.Start).ToListAsync();
            
            
            result.TotalCount = logs.Count;
            result.Logs = logs.Skip(model.From).Take(model.Count).ToList();
            
            
            return result;
        }
    }
}