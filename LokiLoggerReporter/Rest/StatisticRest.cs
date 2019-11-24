using System.Threading.Tasks;
using lokiloggerreporter.Hubs;
using lokiloggerreporter.Services.Implementation;
using lokiloggerreporter.ViewModel.Statistic;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> GetStatistic([FromBody] RestAnalyzeRequestModel model)
        {
            return Ok(await RestAnalyzeService.GetEndPointUsageStatistic(model));
        }
    }
}