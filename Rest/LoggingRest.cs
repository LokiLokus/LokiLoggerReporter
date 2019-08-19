using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using lokiloggerreporter.Models;
using lokiloggerreporter.ViewModel;
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

		[HttpPost("Log/")]
		public async Task<ActionResult> SaveLog([FromBody]SendedLogModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					Source source = DatabaseCtx.Sources.SingleOrDefault(x => x.SourceId == model.SourceId);
					if (source == null) return BadRequest("Source not found");
					if (source.Secret == model.SourceSecret)
					{
						if (model.Logs != null)
						{
							var dbLogs = model.Logs.Select(d =>
							new Log{
								Class = d.Class,
								Data = d.Data,
								Line = d.Line,
								Exception = d.Exception,
								Message = d.Message,
								Method = d.Method,
								Source = source,
								Time = d.Time,
								LogLevel = d.LogLevel,
								LogTyp = d.LogTyp,
								SourceId = source.SourceId,
								ThreadId = d.ThreadId
							});
							DatabaseCtx.Logs.AddRange(dbLogs);
							await DatabaseCtx.SaveChangesAsync();
							return Ok();
						}
					}
					else
					{
						return BadRequest("Secret not Correct");
					}
				}
				else
				{
					return BadRequest(ModelState);
				}
				return BadRequest();
			}
			catch (Exception e)
			{
				return BadRequest();
			}

		}

		[HttpGet("GetTime")]
		public ActionResult GetTime()
		{
			return Ok(DateTime.Now);
		}

		
		[HttpGet("GetLogBySourceDate/{source}/{from}&{to}")]
		public async Task<ActionResult> GetLogBySource([FromRoute] string source,[FromRoute]DateTime from,[FromRoute] DateTime to)
		{
			/*return Ok(DatabaseCtx.Logs.Where(x => x.Time >= from && x.Time <= to)
				.Where(x => x.Name.ToLower().Contains(source.ToLower())).OrderByDescending(x => x.Time).ToList()); //.ToList());*/
			throw new NotImplementedException();
		}
		
		[HttpGet("GetLogBySource/{source}/{offset}-{take}")]
		public async Task<ActionResult> GetLogBySource([FromRoute] string source,[FromRoute]int offset = 0,[FromRoute] int take = 100)
		{
			/*return Ok(DatabaseCtx.Logs.Where(x => x.Name.ToLower().Contains(source.ToLower())).Skip(offset).OrderByDescending(x => x.Time).Take(take).ToList());*/
			throw new NotImplementedException();
		}
	}
}