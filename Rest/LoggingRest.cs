using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using lokiloggerreporter.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace lokiloggerreporter.Rest {
	[ApiController]
	[Route("api/Logging")]
	public class LoggingRest :Controller{
		public DatabaseCtx DatabaseCtx { get; set; }

		public LoggingRest(DatabaseCtx dbCtx)
		{
			DatabaseCtx = dbCtx;
		}

		[HttpPost("Log")]
		public async Task<ActionResult> SaveLog([FromBody] List<Log> logs)
		{
			try
			{
				if (logs != null)
				{
					DatabaseCtx.AddRange(logs);
					await DatabaseCtx.SaveChangesAsync();
					return Ok();
				}
				return BadRequest();
			}
			catch (Exception e)
			{
				return BadRequest();
			}

		}

		[HttpGet("GetLogBySource/{source}/{offset}-{take}")]
		public async Task<ActionResult> GetLogBySource([FromRoute] string source,[FromRoute]int offset = 0,[FromRoute] int take = 100)
		{
			return Ok(DatabaseCtx.Logs.Where(x => x.Name.ToLower().Contains(source.ToLower())).Skip(offset).Take(take).OrderByDescending(x => x.Time).ToList());
			HttpClient t = new HttpClient();
			var d = await t.GetStringAsync("https://llogger.hopfenspace.org/api/Logging/GetLog");
			var result = JsonConvert.DeserializeObject < List<Log>>(d);
			//string data = @;	
			return Ok(result.Where(x => x.Name.ToLower().Contains(source.ToLower())));
		}
	}
}