using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lokiloggerreporter.Extensions;
using lokiloggerreporter.Models;
using lokiloggerreporter.ViewModel.Statistic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

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
            var logs = DatabaseCtx.Logs.Where(x => x.SourceId == sourceId && x.LogTyp == LogTyp.RestCall).Take(100).Include(x => x.WebRequest).Select(x => x.WebRequest);
            
            
            EndPointUsage result = new EndPointUsage();
            result.EndPoint = "/";
            IEnumerable<List<string>> endpoints = ObtainEndPoints(logs);
            var deep = endpoints.FirstOrDefault()?.Count;
            foreach (List<string> endpoint in endpoints)
            {
                int i = 0;
                foreach (string pathPart in endpoint)
                {

                    i++;
                }
            }
            
            //Obtain all Endpoints as Tree Structure
            //
            return result;
        }

        private IEnumerable<List<string>> ObtainEndPoints(IQueryable<WebRequest> logs)
        {
            IEnumerable<List<string>> endpoints = logs.Select(x => x.Path).Select(x => x.Split("/",StringSplitOptions.None).ToList());
            int data = endpoints.Count();
            var obtainEndPoints = endpoints.ToList();
            int deepest = obtainEndPoints.Max(x => x.Count);

            Parallel.ForEach(obtainEndPoints, x =>
            {
                int addCount = deepest - x.Count;
                for (int i = 0; i < addCount; i++)
                {
                    x.Add(null);
                }
            });
            
            return obtainEndPoints;
        }
    }
}