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
					Count = x.Where(d => d.Time >= Time.Now.Add(SettingsService.Get<TimeSpan>("SourceLogCountTime"))).Sum(f => 1),
					AllCount = x.Sum(f => 1),
					Level = LogLevelExtension.Levels().Select(l =>
						new
						{
							Level = l,
							Count = x.Where(d => d.LogLevel == l && d.Time >= Time.Now.Add(SettingsService.Get<TimeSpan>("SourceLogCountTime"))).Sum(f => 1)
						}
					).OrderBy(f => f.Level),
					Typ = LogTypExtension.Typs.Select(z =>
						new {
							Typ = z,
							Count = x.Where(d => d.LogTyp == z && d.Time >= Time.Now.Add(SettingsService.Get<TimeSpan>("SourceLogCountTime"))).Sum(f => 1)
						}).OrderBy(f => f.Typ)
				});
			return Ok(data);
		}
	}
}