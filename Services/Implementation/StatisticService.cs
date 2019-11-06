using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using lokiloggerreporter.Extensions;
using lokiloggerreporter.Hubs;
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
        public Dictionary<string,Stopwatch> Stopwatches { get; set; } = new Dictionary<string, Stopwatch>();

        public StatisticService(DatabaseCtx databaseCtx)
        {
            DatabaseCtx = databaseCtx;
        }

        public async Task<EndPointUsage> GetEndPointUsageStatistic(RequestModel model)
        {
            //I know Joins are nice, but this is faster

            Stopwatches["ObtainLogs"] = Stopwatch.StartNew();
            var logs = DatabaseCtx.WebRequest.Where(x => DatabaseCtx.Logs.Where(z => z.SourceId == model.SourceId && 
                                                                                     (model.FromTime == null || x.Start >= model.FromTime) &&
                                                                                     (model.ToTime == null || x.Start <= model.ToTime)&&
                                                                                     z.LogTyp == LogTyp.RestCall
                                                                                     ).Any(z => z.WebRequestId == x.WebRequestId)).ToList();
            
            Stopwatches["ObtainLogs"].Stop();
            EndPointUsage result = new EndPointUsage();
            result.EndPoint = "";
            
            
            Stopwatches["ObtainEndPoints"] = Stopwatch.StartNew();
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
            Stopwatches["ObtainEndPoints"].Stop();
            
            if(model.FromTime == null)
                model.FromTime = logs.OrderBy(x => x.Start).FirstOrDefault()?.Start;
            if(model.ToTime == null)
                model.ToTime = logs.OrderByDescending(x => x.Start).FirstOrDefault()?.Start;

            TimeSpan analyzeSpan = TimeSpan.Zero;
            List<DateTime> fromTimes = new List<DateTime>();
            if(model.FromTime != null){
                analyzeSpan = ((DateTime) model.ToTime - (DateTime) model.FromTime) / 50;
                int i = 1;
                do
                {
                    fromTimes.Add((DateTime)model.FromTime+(analyzeSpan* i));
                    i++;
                } while (fromTimes.Last() <= model.ToTime);
            }
            
            Stopwatches["AddLogs"] = Stopwatch.StartNew();
            result = AddLogToLogs(logs, result);
            GetNodes(result,true);
            Stopwatches["AddLogs"].Stop();
            
            
            Stopwatches["LeaveOb"] = Stopwatch.StartNew();
            foreach (var endPointUsage in _Leaves)
            {
                if(endPointUsage.WebRequests.Count != 0){
                    endPointUsage.AverageRequestTime =(int) endPointUsage.WebRequests.DefaultIfEmpty().Average(x => (x.End - x.Start).Ticks);
                    endPointUsage.MaximumRequestTime =(int) endPointUsage.WebRequests.DefaultIfEmpty().Max(x => (x.End - x.Start).Ticks);
                    endPointUsage.MinimumRequestTime =(int) endPointUsage.WebRequests.DefaultIfEmpty().Min(x => (x.End - x.Start).Ticks);
                    endPointUsage.MedianRequestTime =(int) endPointUsage.WebRequests.DefaultIfEmpty().Median(x => (x.End - x.Start).Ticks);
                    endPointUsage.AbsoluteRequestTime = (long) endPointUsage.WebRequests.DefaultIfEmpty().Sum(x => (x.End - x.Start).Ticks);
                    endPointUsage.RequestCount = endPointUsage.WebRequests.Count;
                    endPointUsage.ErrorCount = endPointUsage.WebRequests.Where(x => x.StatusCode >= 400).Count();
                    if (model.FromTime != null)
                    {
                        Parallel.ForEach(fromTimes,
                            x =>
                            {
                                endPointUsage._concurrentRequests.Add(CalculateLeaveTimeSteps(x, analyzeSpan,
                                    endPointUsage.WebRequests));
                            });
                    }
                }
                endPointUsage.Processed = true;
            }
            Stopwatches["LeaveOb"].Stop();
            Stopwatches["NodePro"] = Stopwatch.StartNew();

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
                    tmp.AbsoluteRequestTime = (long) tmp.EndPoints.DefaultIfEmpty().Sum(x => x.AbsoluteRequestTime);
                    tmp.Processed = true;
                    var requests = tmp.EndPoints.SelectMany(x => x._concurrentRequests);
                    if (model.FromTime != null)
                    {
                        Parallel.ForEach(fromTimes,
                            x =>
                            {
                                tmp._concurrentRequests.Add(CalculateNodeTimeSteps(x, analyzeSpan,
                                    requests));
                            });
                    }
                }
            }
            Stopwatches["NodePro"].Stop();
            _Nodes.ForEach(x => x.Processed = false);
            result.EndPoint = "";
            result.Processed = true;
            while (_Nodes.Any(x => !x.Processed))
            {
                var tmpNodes = _Nodes.Where(x => !x.Processed && x.Parent.Processed);
                foreach (var tmpNode in tmpNodes)
                {
                    tmpNode.EndPoint = tmpNode.Parent.EndPoint + "/" + tmpNode.EndPoint;
                    tmpNode.Processed = true;
                }
            }
            foreach (var keyValuePair in Stopwatches)
            {
                Console.WriteLine($"{keyValuePair.Key} \t {keyValuePair.Value.ElapsedMilliseconds}");
            }
            Console.WriteLine("LogsCount:     " + logs.Count);
            return result;
        }

        private RequestAnalyzeModel CalculateNodeTimeSteps(DateTime from, TimeSpan span, IEnumerable<RequestAnalyzeModel> requests)
        {
            DateTime toTime = from + span;
            RequestAnalyzeModel result = new RequestAnalyzeModel();
            var requestsInTIme = requests.Where(x => x.FromTime >= from && x.ToTime <= toTime);
            result.FromTime = from;
            result.ToTime = toTime;
            result.ErrorCount = requestsInTIme.Sum(x => x.ErrorCount);
            result.RequestCount = requestsInTIme.Sum(x => x.RequestCount);
            result.InterestingRequestModel = requestsInTIme.SelectMany(x => x.InterestingRequestModel).Take(50).ToList();
            return result;
        }

        private RequestAnalyzeModel CalculateLeaveTimeSteps(DateTime from, TimeSpan span, List<WebRequest> webRequests)
        {
            DateTime toTime = from + span;
            RequestAnalyzeModel result = new RequestAnalyzeModel();
            var requestsInTIme = webRequests.Where(x => x.Start >= from && x.Start <= toTime);
            result.FromTime = from;
            result.ToTime = toTime;
            result.ErrorCount = requestsInTIme.Count(x => !x.IsStatusCodeSucceded);
            result.RequestCount = requestsInTIme.Count();
            result.InterestingRequestModel = requestsInTIme.Where(x => !x.IsStatusCodeSucceded).Take(10).ToList();
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
            IEnumerable<List<string>> endpoints = logs.DistinctBy(x => x.Path).Select(x => x.Path).Select(x =>{
                if(x == null) return new List<string>();
                return x.Split("/", StringSplitOptions.None).ToList();
            });
            return endpoints;
        }
    }
}














