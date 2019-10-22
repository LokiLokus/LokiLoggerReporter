using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lokiloggerreporter.Extensions;
using lokiloggerreporter.Models;
using lokiloggerreporter.ViewModel.Statistic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;

namespace lokiloggerreporter.Services.Implementation
{
    public class StatisticService
    {
        public DatabaseCtx DatabaseCtx { get; set; }
        public StatisticService(DatabaseCtx databaseCtx)
        {
            DatabaseCtx = databaseCtx;
        }
        
        

        public EndPointUsage GetEndPointUsageStatistic(string sourceId)
        {
            

            IQueryable<IGrouping<string, Log>> logData = DatabaseCtx.Logs.Where(x => x.SourceId == sourceId || x.LogTyp == LogTyp.RestCall)
                .Include(x => x.WebRequest).GroupBy(x => x.WebRequest.Path);
            
            
            
            
            var logs = DatabaseCtx.Logs.Where(x => x.SourceId == sourceId && x.LogTyp == LogTyp.RestCall).Take(100).Include(x => x.WebRequest).Select(x => x.WebRequest);
            EndPointUsage result = new EndPointUsage();
            result.EndPoint = "";
            IEnumerable<List<string>> endpoints = ObtainEndPoints(logs);
            foreach (List<string> endpoint in endpoints)
            {
                EndPointUsage tmp = result;
                for (int j = 1; j < endpoint.Count; j++)
                {
                    var ttmp = tmp.EndPoints.SingleOrDefault(x => x.EndPoint == endpoint[j]);
                    if (ttmp == null)
                    {
                        ttmp = new EndPointUsage()
                        {
                            EndPoint = endpoint[j],
                            Parent = tmp
                        };
                        tmp.EndPoints.Add(ttmp);
                    }

                    tmp = ttmp;
                }
            }
            
            result = AddLogToLogs(logs, result);
            
            
            
            return result;
        }

        private EndPointUsage AddLogToLogs(IQueryable<WebRequest> logs,EndPointUsage endPointUsage)
        {
            foreach (var log in logs)
            {
                var pathSplit = log.Path.Split("/").Skip(1);
                var tmp = endPointUsage;
                foreach (var pathPart in pathSplit)
                {
                     tmp = tmp.EndPoints.Single(x => x.EndPoint == pathPart);
                }
                tmp.WebRequests.Add(log);
            }

            return endPointUsage;
        }
        

        private IEnumerable<List<string>> ObtainEndPoints(IQueryable<WebRequest> logs)
        {
            IEnumerable<List<string>> endpoints = logs.DistinctBy(x => x.Path).Select(x => x.Path).Select(x => x.Split("/",StringSplitOptions.None).ToList());
            return endpoints;
        }
    }
}














