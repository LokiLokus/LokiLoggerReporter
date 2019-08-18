using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lokiloggerreporter.Extensions;
using lokiloggerreporter.Models;
using lokiloggerreporter.Services;
using lokiloggerreporter.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Adapter.Internal;

namespace lokiloggerreporter.Rest {
	[Route("api/Source")]
	[ApiController]
	public class SourceRest : Controller {
		public SourceRest(DatabaseCtx dctx, ISettingsService settingsService)
		{
			DatabaseContext = dctx;
			SettingsService = settingsService;
		}

		public DatabaseCtx DatabaseContext { get; set; }
		public ISettingsService SettingsService { get; set; }


		[HttpGet("All")]
		public ActionResult GetAll()
		{
			var data = DatabaseContext.Sources.ToList();
			return Ok(data);
		}

		[HttpPost("New")]
		public async Task<ActionResult> NewSource([FromBody] Source model)
		{
			if (ModelState.IsValid)
			{
				if (!DatabaseContext.Sources.Any(x =>
					x.Name == model.Name && x.Version == model.Version && x.Tag == model.Tag))
				{
					DatabaseContext.Sources.Add(model);
					await DatabaseContext.SaveChangesAsync();
					return Ok(model);
				}
				else
				{
					return BadRequest(OperationResult.Failed("Name", "Same Source already exists"));
				}
			}
			else
			{
				return BadRequest(ModelState);
			}
		}
		
		[HttpPut("Update/{sourceId}")]
		public async Task<ActionResult> UpdateSource([FromRoute]string sourceId ,[FromBody] Source model)
		{
			if (ModelState.IsValid)
			{
				Source source = DatabaseContext.Sources.SingleOrDefault(x => x.SourceId == sourceId);
				if (source != null)
				{
					source.Description = model.Description;
					source.Name = model.Name;
					source.Secret = model.Secret;
					source.Tag = model.Tag;
					source.Version = model.Version;
					
					await DatabaseContext.SaveChangesAsync();
					return Ok(model);
				}
				else
				{
					return BadRequest(OperationResult.Failed("Name", "Source doesn't exists"));
				}
			}
			else
			{
				return BadRequest(ModelState);
			}
		}

		[HttpGet("LastStatistic")]
		public ActionResult GetAllSources()
		{
			/*
			var names = DatabaseContext.Logs.DistinctBy(x => x.Name);
			
			List<Task> runner = new List<Task>();
			
			var data = DatabaseContext.Logs.Where(d =>
				d.Time >= Time.Now.Add(SettingsService.Get<TimeSpan>("SourceLogCountTime"))).GroupBy(x => x.Name)
				.Select(x => new
				{
					Source = x.Key,
					Count = x.Sum(f => 1),
					AllCount = DatabaseContext.Logs.Where(f => f.Name == x.Key).Sum(f => 1),
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
				AllCount = DatabaseContext.Logs.Where(f => f.Name == x.Name).Sum(f => 1),
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
			return Ok(tmp);*/
			throw new NotImplementedException();
		}
	}
}