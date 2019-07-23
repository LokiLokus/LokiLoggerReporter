using System;
using System.Collections.Generic;
using System.Linq;
using lokiloggerreporter.Extensions;
using lokiloggerreporter.Models;
using lokiloggerreporter.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;

namespace lokiloggerreporter.Rest {
	[Route("api/Source")]
	[ApiController]
	public class SourceRest :Controller {
		public DatabaseCtx ctx { get; set; }
		public ISettingsService SettingsService { get; set; }
		
		
		public SourceRest(DatabaseCtx dctx, ISettingsService settingsService)
		{
			ctx = dctx;
			SettingsService = settingsService;
		}


		[HttpGet("All")]
		public ActionResult GetAllSources()
		{
			var data = ctx.Logs.GroupBy(x => x.Name)
				.Select(x => new
				{
					Source = x.Key,
					Count = x.Count(d => d.Time >= Time.Now.Add(SettingsService.Get<TimeSpan>("SourceLogCountTime"))),
					AllCount = x.Count(),
					Level = LogLevelExtension.Levels().Select(l =>
						new
						{
							Level = l,
							Count = x.Count(d => d.LogLevel == l && d.Time >= Time.Now.Add(SettingsService.Get<TimeSpan>("SourceLogCountTime")))
						}
					),
					Typ = LogTypExtension.Typs.Select(z =>
						new {
							Typ = z,
							Count = x.Count(d => d.LogTyp == z && d.Time >= Time.Now.Add(SettingsService.Get<TimeSpan>("SourceLogCountTime")))
						})
				});
			return Ok(data);
		}
	}
}