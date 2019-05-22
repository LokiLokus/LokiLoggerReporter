using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lokiloggerreporter.Database;
using lokiloggerreporter.Database.Model;
using Microsoft.AspNetCore.Mvc;

namespace lokiloggerreporter.Controllers {
	[ApiController]
	[Route("api/Logging")]
	public class LoggingController :Controller{
		private readonly DatabaseCtx _dbContext;
		public LoggingController(DatabaseCtx dbContext)
		{
			_dbContext = dbContext;
		}

		
		[HttpGet("GetLogs")]
		public ActionResult<Log> GetAllLogs()
		{
			return Ok(_dbContext.Logs.ToList());
		}
		
		[HttpPost("Log")]
		public async Task<IActionResult> Log([FromBody] List<Log> model)
		{
			try
			{
				if (model != null)
				{
					_dbContext.AddRange(model);
					await _dbContext.SaveChangesAsync();
					return Ok();
				}
				return BadRequest();
			}
			catch (Exception e)
			{
				return BadRequest();
			}
		}

	}
}