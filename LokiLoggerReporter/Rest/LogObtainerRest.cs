using System.Threading.Tasks;
using lokiloggerreporter.Hubs;
using lokiloggerreporter.Services.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace lokiloggerreporter.Rest
{
    public class LogObtainerRest:Controller
    {
        public LogsObtainerService LogsObtainerService { get; set; }

        public LogObtainerRest(LogsObtainerService service)
        {
            LogsObtainerService = service;
        }

        [HttpPost("SearchLogs")]
        public async Task<IActionResult> SearchLogs([FromBody] RequestModel model)
        {
            return Ok(await LogsObtainerService.ObtainLogs(model));
        }
    }
}