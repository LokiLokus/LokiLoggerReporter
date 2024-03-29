using lokiloggerreporter.Services.Implementation;
using lokiloggerreporter.ViewModel.Statistic;
using Microsoft.AspNetCore.Mvc;

namespace lokiloggerreporter.Rest
{
    public class RestCallUsageRest:Controller
    {
        public RestAnalyzeService RestAnalyzeService { get; set; }

        public RestCallUsageRest(RestAnalyzeService service)
        {
            RestAnalyzeService = service;
        }
        
        [HttpGet("EndPointsFromSource/{sourceId}")]
        public IActionResult GetStatisticFromSource([FromRoute] string sourceId)
        {
            return Ok();
            //return Ok(StatisticService.GetEndPointUsageStatistic(sourceId));
        }
    }
}