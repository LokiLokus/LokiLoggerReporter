using System.Threading.Tasks;
using lokiloggerreporter.Hubs;
using lokiloggerreporter.Services.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace lokiloggerreporter.Rest
{
    public class StatisticRest: Controller
    {
        public RestAnalyzeService RestAnalyzeService { get; set; }

        public StatisticRest(RestAnalyzeService restAnalyzeService)
        {
            RestAnalyzeService = restAnalyzeService;
        }

        [HttpPost("GetStatistic")]
        public async Task<IActionResult> GetStatistic([FromBody] RequestModel model)
        {
            return Ok(await RestAnalyzeService.GetEndPointUsageStatistic(model));
        }
    }
}