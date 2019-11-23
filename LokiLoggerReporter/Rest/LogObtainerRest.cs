using System.Threading.Tasks;
using lokiloggerreporter.Hubs;
using lokiloggerreporter.Services.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lokiloggerreporter.Rest
{
    [Authorize]
    public class LogObtainerRest:Controller
    {
        public LogsObtainerService LogsObtainerService { get; set; }

        public LogObtainerRest(LogsObtainerService service)
        {
            LogsObtainerService = service;
        }

        [HttpPost("SearchWebRequests")]
        public async Task<IActionResult> SearchWebRequests([FromBody] WebRequestModel model)
        {
            return Ok(await LogsObtainerService.ObtainWebRequests(model));
        }
    }
}