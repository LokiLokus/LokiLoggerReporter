using System.Threading.Tasks;
using lokiloggerreporter.Hubs;
using lokiloggerreporter.Services.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace lokiloggerreporter.Rest
{
    public class StatisticRest: Controller
    {
        public StatisticService StatisticService { get; set; }

        public StatisticRest(StatisticService statisticService)
        {
            StatisticService = statisticService;
        }

        [HttpPost("GetStatistic")]
        public async Task<IActionResult> GetStatistic([FromBody] RequestModel model)
        {
            return Ok(await StatisticService.GetEndPointUsageStatistic(model));
        }
    }
}