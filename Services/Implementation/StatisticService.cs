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

        public EndPointUsage GetEndPointUsageStatistic(IQueryable<Log> inputLogs)
        {
            //IQueryable<IGrouping<string, Log>> logData = inputLogs.Include(x => x.WebRequest).GroupBy(x => x.WebRequest.Path);

            var logs = inputLogs.Select(x => x.WebRequest).ToList();
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
            GetNodes(result,true);
            foreach (var endPointUsage in _Leaves)
            {
                endPointUsage.AverageRequestTime =(int) endPointUsage.WebRequests.DefaultIfEmpty().Average(x => (x.End - x.Start).Ticks);
                endPointUsage.MaximumRequestTime =(int) endPointUsage.WebRequests.DefaultIfEmpty().Max(x => (x.End - x.Start).Ticks);
                endPointUsage.MinimumRequestTime =(int) endPointUsage.WebRequests.DefaultIfEmpty().Min(x => (x.End - x.Start).Ticks);
                endPointUsage.MedianRequestTime =(int) endPointUsage.WebRequests.DefaultIfEmpty().Median(x => (x.End - x.Start).Ticks);
                endPointUsage.RequestCount = endPointUsage.WebRequests.Count;
                endPointUsage.ErrorCount = endPointUsage.WebRequests.Where(x => x.StatusCode >= 400).Count();
                
                endPointUsage.Processed = true;
            }

            GetNodes(result, false);

            while (_Nodes.Any(x => !x.Processed))
            {
                IEnumerable<EndPointUsage> nodes = _Nodes.Where(x => !x.Processed && x.EndPoints.All(z => z.Processed));
                foreach (EndPointUsage tmp in nodes)
                {
                    tmp.RequestCount = tmp.EndPoints.Sum(x => x.RequestCount);
                    tmp.ErrorCount = tmp.EndPoints.Sum(x => x.RequestCount);
                    tmp.AverageRequestTime =(int) tmp.EndPoints.DefaultIfEmpty().Average(x => x.AverageRequestTime);
                    tmp.MaximumRequestTime = (int)tmp.EndPoints.DefaultIfEmpty().Max(x => x.MaximumRequestTime);
                    tmp.MinimumRequestTime = (int)tmp.EndPoints.DefaultIfEmpty().Min(x => x.MinimumRequestTime);
                    tmp.MedianRequestTime = (int)tmp.EndPoints.DefaultIfEmpty().Median(x => x.MedianRequestTime);
                    tmp.Processed = true;
                }
            }
            
            return result;
        }
        
        

        private List<EndPointUsage> _Leaves = new List<EndPointUsage>();
        private List<EndPointUsage> _Nodes = new List<EndPointUsage>();

        
        
        private void GetNodes(EndPointUsage endPointUsage,bool onlyLeaves)
        {
            if(!onlyLeaves)_Nodes.Add(endPointUsage);
            if (endPointUsage.EndPoints.Count == 0)
            {
                if (onlyLeaves)
                    _Leaves.Add(endPointUsage);
                else
                {
                    _Nodes.Add(endPointUsage);
                }
            }
            else
            {
                foreach (var pointUsage in endPointUsage.EndPoints)
                {
                    GetNodes(pointUsage,onlyLeaves);
                }
            }
        }



    private EndPointUsage AddLogToLogs(IEnumerable<WebRequest> logs,EndPointUsage endPointUsage)
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
        

        private IEnumerable<List<string>> ObtainEndPoints(IEnumerable<WebRequest> logs)
        {
            IEnumerable<List<string>> endpoints = logs.DistinctBy(x => x.Path).Select(x => x.Path).Select(x => x.Split("/",StringSplitOptions.None).ToList());
            return endpoints;
        }
    }
}














