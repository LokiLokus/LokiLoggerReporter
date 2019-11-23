using System;
using System.Collections;
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
    public class RestAnalyzeService
    {
        public DatabaseCtx DatabaseCtx { get; set; }
        public Dictionary<string,Stopwatch> Stopwatches { get; set; } = new Dictionary<string, Stopwatch>();

        public RestAnalyzeService(DatabaseCtx databaseCtx)
        {
            DatabaseCtx = databaseCtx;
        }

        public async Task<EndPointUsage> GetEndPointUsageStatistic(RestAnalyzeRequestModel model)
        {
            //I know Joins are nice, but this is faster

            Stopwatches["ObtainLogs"] = Stopwatch.StartNew();
            var logs = await DatabaseCtx.WebRequest.Where(x => DatabaseCtx.Logs.Where(z => z.SourceId == model.SourceId && 
                                                                                     (model.FromTime == null || x.Start >= model.FromTime) &&
                                                                                     (model.ToTime == null || x.Start <= model.ToTime)&&
                                                                                     (model.Ignore404 == false || x.StatusCode != 404)&&
                                                                                     z.LogTyp == LogTyp.RestCall
                                                                                     ).Any(z => z.WebRequestId == x.WebRequestId)).ToListAsync();
            
            Stopwatches["ObtainLogs"].Stop();
            EndPointUsage result = new EndPointUsage();
            result.EndPoint = "";
            
            
            Stopwatches["ObtainEndPoints"] = Stopwatch.StartNew();
            IEnumerable<List<string>> endpoints = ObtainEndPoints(logs);
            Stopwatches["ObtainInnerEndPoints"] = Stopwatch.StartNew();
            var dasd = endpoints.ToList();
            foreach (List<string> endpoint in dasd)
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
                    ttmp.EndPoints = ttmp.EndPoints.OrderBy(x => x.EndPoint).ToList();

                    tmp = ttmp;
                }
            }

            Stopwatches["ObtainInnerEndPoints"].Stop();
            Stopwatches["ObtainEndPoints"].Stop();
            var firstLog = logs.OrderBy(x => x.Start).FirstOrDefault()?.Start;
            var lastLog = logs.OrderByDescending(x => x.Start).FirstOrDefault()?.Start;;

            if(model.FromTime == null)
                model.FromTime = firstLog;
            if (model.ToTime == null)
                model.ToTime = lastLog;

            TimeSpan analyzeSpan = TimeSpan.Zero;
            List<DateTime> fromTimes = new List<DateTime>();
            if(model.FromTime != null){
                analyzeSpan = ((DateTime) model.ToTime - (DateTime) model.FromTime) / model.Resolution;
                int i = 1;
                do
                {
                    fromTimes.Add((DateTime)model.FromTime+(analyzeSpan* i));
                    i++;
                } while (fromTimes.Last() < model.ToTime);
            }
            
            Stopwatches["AddLogs"] = Stopwatch.StartNew();
            result = AddLogToLogs(logs, result);
            GetNodes(result,true);
            Stopwatches["AddLogs"].Stop();
            
            Stopwatches["LeaveOb"] = Stopwatch.StartNew();
            Parallel.ForEach(_Leaves, endPointUsage =>
            {
                
                if (model.FromTime != null)
                {
                    
                    foreach (var x in fromTimes)
                    {
                        var tmpData = CalculateLeaveTimeSteps(x, analyzeSpan,endPointUsage.WebRequests);
                        endPointUsage.TimeSlots.Add(tmpData);
                    }
                    /*
                                        Dictionary<DateTime,List<WebRequest>> preorderdCache = new Dictionary<DateTime, List<WebRequest>>();
                                        int splitSize = (int)(model.Resolution / 10);
                                        foreach (var dateTime in fromTimes.SplitList(splitSize))
                                        {
                                            preorderdCache.Add(dateTime.FirstOrDefault(),endPointUsage.WebRequests.Where(x => x.Start >= dateTime.FirstOrDefault() && x.Start <= dateTime.Last()).ToList());
                                        }
                    var first = endPointUsage.TimeSlots.First();
                    
                    var toRemove =  new List<RequestAnalyzeModel>();
                    for (int i = 1; i < endPointUsage.TimeSlots.Count-1; i++)
                    {
                        if (!first.AnyRequest && !endPointUsage.TimeSlots[i].AnyRequest)
                        {
                            if(!endPointUsage.TimeSlots[i + 1].AnyRequest){
                                toRemove.Add(endPointUsage.TimeSlots[i]);
                            }
                        }
                        first = endPointUsage.TimeSlots[i];
                    }
                    endPointUsage.TimeSlots.RemoveAll(x => toRemove.Contains(x));*/
                }

                endPointUsage.Processed = true;
            });
            Stopwatches["LeaveOb"].Stop();
            Stopwatches["NodePro"] = Stopwatch.StartNew();

            GetNodes(result, false);

            while (_Nodes.Any(x => !x.Processed))
            {
                List<EndPointUsage> nodes = _Nodes.Where(x => !x.Processed && x.EndPoints.All(z => z.Processed)).ToList();
                nodes.ForEach(tmp =>
                {
                    tmp.Processed = true;
                    var requests = tmp.EndPoints.SelectMany(x => x.TimeSlots).ToList();
                    if (model.FromTime != null)
                    {
                        foreach (var x in fromTimes)
                        {
                            var tmpData = CalculateNodeTimeSteps(x, analyzeSpan,requests);
                            tmp.TimeSlots.Add(tmpData);
                        }
                        /*
                        Dictionary<DateTime,List<RequestAnalyzeModel>> preorderdCache = new Dictionary<DateTime, List<RequestAnalyzeModel>>();
                        int splitSize = (int)(model.Resolution/10);
                        foreach (var dateTime in fromTimes.SplitList(splitSize))
                        {
                            preorderdCache.Add(dateTime.FirstOrDefault(),requests.Where(x => x.FromTime >= dateTime.FirstOrDefault() && x.FromTime <= dateTime.Last()).ToList());
                        }
                        
                        
                        var first = tmp.TimeSlots.First();
                        
                        var toRemove =  new List<RequestAnalyzeModel>();
                        for (int i = 1; i < tmp.TimeSlots.Count-1; i++)
                        {
                            if (!first.AnyRequest && !tmp.TimeSlots[i].AnyRequest)
                            {
                                if (!tmp.TimeSlots[i + 1].AnyRequest)
                                {
                                    toRemove.Add(tmp.TimeSlots[i]);
                                }
                            }

                            first = tmp.TimeSlots[i];
                        }
                        tmp.TimeSlots.RemoveAll(x => toRemove.Contains(x));*/
                    }
                });
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
            Console.WriteLine("LogsCount:     " + logs.Count());
            Console.WriteLine("");
            result.EndPoint = "/";
            return result;
        }

        private RequestAnalyzeModel CalculateNodeTimeSteps(DateTime from, TimeSpan span, IEnumerable<RequestAnalyzeModel> requests)
        {
            DateTime toTime = from + span;
            RequestAnalyzeModel result = new RequestAnalyzeModel();
            var requestsInTime = requests.Where(x => x.FromTime >= from && x.ToTime <= toTime).ToList();
            result.FromTime = from;
            result.ToTime = toTime;
            result.Request100Count = requestsInTime.Sum(x => x.Request100Count);
            result.Request200Count = requestsInTime.Sum(x => x.Request200Count);
            result.Request300Count = requestsInTime.Sum(x => x.Request300Count);
            result.Request400Count = requestsInTime.Sum(x => x.Request400Count);
            result.Request500Count = requestsInTime.Sum(x => x.Request500Count);
            result.Request900Count = requestsInTime.Sum(x => x.Request900Count);
            if (requestsInTime.Any())
            {
                result.AverageRequestTime = (int) requestsInTime.DefaultIfEmpty().Average(x => x.AverageRequestTime);
                result.MinimumRequestTime = (int) requestsInTime.DefaultIfEmpty().Min(x => x.MinimumRequestTime);
                result.MaximumRequestTime = (int) requestsInTime.DefaultIfEmpty().Max(x => x.MaximumRequestTime);
                result.MedianRequestTime = (int) requestsInTime.DefaultIfEmpty().Median(x => x.MedianRequestTime);
                result.AbsoluteRequestTime = (int) requestsInTime.DefaultIfEmpty().Sum(x => x.AbsoluteRequestTime);
            }

            return result;
        }

        private RequestAnalyzeModel CalculateLeaveTimeSteps(DateTime from, TimeSpan span, List<WebRequest> webRequests)
        {
            DateTime toTime = from + span;
            var requestsInTime = webRequests.Where(x => x.Start >= from && x.Start <= toTime).ToList();

            RequestAnalyzeModel result = new RequestAnalyzeModel
            {
                FromTime = @from,
                ToTime = toTime,
                Request100Count = requestsInTime.Count(x => x.StatusCode >= 0 && x.StatusCode <= 199),
                Request200Count = requestsInTime.Count(x => x.StatusCode >= 200 && x.StatusCode <= 299),
                Request300Count = requestsInTime.Count(x => x.StatusCode >= 300 && x.StatusCode <= 399),
                Request400Count = requestsInTime.Count(x => x.StatusCode >= 400 && x.StatusCode <= 499),
                Request500Count = requestsInTime.Count(x => x.StatusCode >= 500 && x.StatusCode <= 599),
                Request900Count = requestsInTime.Count(x => x.StatusCode >= 600)
            };
            if (requestsInTime.Any())
            {
                result.AverageRequestTime = (int)requestsInTime.DefaultIfEmpty().Average(x => (x.End-x.Start).Ticks);
                result.MinimumRequestTime = (int)requestsInTime.DefaultIfEmpty().Min(x => (x.End-x.Start).Ticks);
                result.MaximumRequestTime = (int)requestsInTime.DefaultIfEmpty().Max(x => (x.End-x.Start).Ticks);
                result.MedianRequestTime = (int)requestsInTime.DefaultIfEmpty().Median(x => (x.End-x.Start).Ticks);
                result.AbsoluteRequestTime = (int)requestsInTime.DefaultIfEmpty().Sum(x => (x.End-x.Start).Ticks);
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
        

        private List<List<string>> ObtainEndPoints(List<WebRequest> logs)
        {
            IEnumerable<List<string>> endpoints = logs.DistinctBy(x => x.Path).Select(x => x.Path).Select(x =>{
                if(x == null) return new List<string>();
                return x.Split("/", StringSplitOptions.None).ToList();
            });
            return endpoints.ToList();
        }
    }
}