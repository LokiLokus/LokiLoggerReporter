using System;
using System.Collections.Generic;
using System.Linq;
using lokiloggerreporter.Extensions;
using lokiloggerreporter.Models;
using lokiloggerreporter.Services;
using Microsoft.AspNetCore.Mvc;

namespace lokiloggerreporter.Rest {
	[Route("api/Source")]
	[ApiController]
	public class SourceRest : Controller {
		public SourceRest(DatabaseCtx dctx, ISettingsService settingsService)
		{
			ctx = dctx;
			SettingsService = settingsService;
		}

		public DatabaseCtx ctx { get; set; }
		public ISettingsService SettingsService { get; set; }


		[HttpGet("All")]
		public ActionResult GetAllSources()
		{
			var names = ctx.Logs.DistinctBy(x => x.Name);
			var data = ctx.Logs.Where(d =>
				d.Time >= Time.Now.Add(SettingsService.Get<TimeSpan>("SourceLogCountTime"))).GroupBy(x => x.Name)
				.Select(x => new
				{
					Source = x.Key,
					Count = x.Sum(f => 1),
					AllCount = ctx.Logs.Where(f => f.Name == x.Key).Sum(f => 1),
					Level = LogLevelExtension.Levels().Select(l =>
						new
						{
							Level = l,
							Count = x.Where(d => d.LogLevel == l).Sum(f => 1)
						}
					).OrderBy(f => f.Level),
					Typ = LogTypExtension.Typs.Select(z =>
						new
						{
							Typ = z,
							Count = x.Where(d => d.LogTyp == z).Sum(f => 1)
						}).OrderBy(f => f.Typ)
				});

			var zerodata = names.Where(x => !data.Any(d => d.Source == x.Name)).Select(x => new
			{
				Source = x.Name,
				Count = 0,
				AllCount = ctx.Logs.Where(f => f.Name == x.Name).Sum(f => 1),
				Level = LogLevelExtension.Levels().Select(l =>
					new
					{
						Level = l,
						Count = 0
					}
				).OrderBy(f => f.Level),
				Typ = LogTypExtension.Typs.Select(z =>
					new
					{
						Typ = z,
						Count = 0
					}).OrderBy(f => f.Typ)
			});
			var tmp = data.ToList();
			tmp.AddRange(zerodata);
			return Ok(tmp);
		}
	}
}