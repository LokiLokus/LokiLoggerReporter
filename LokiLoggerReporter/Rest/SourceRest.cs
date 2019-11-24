using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LokiLogger.WebExtension.ViewModel;
using lokiloggerreporter.Extensions;
using lokiloggerreporter.Models;
using lokiloggerreporter.Services;
using lokiloggerreporter.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace lokiloggerreporter.Rest
{
    [Route("api/Source")]
    [ApiController]
    public class SourceRest : Controller
    {
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
            var data = DatabaseContext.Sources.OrderBy(x => x.Name).ToList();
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

                return BadRequest(OperationResult.Fail<bool>("Name", "Same Source already exists").Errors);
            }

            return BadRequest(ModelState);
        }

        [HttpPut("Update/{sourceId}")]
        public async Task<ActionResult> UpdateSource([FromRoute] string sourceId, [FromBody] Source model)
        {
            if (ModelState.IsValid)
            {
                var source = DatabaseContext.Sources.SingleOrDefault(x => x.SourceId == sourceId);
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

                return BadRequest(OperationResult.Fail<bool>("Name", "Source doesn't exists").Errors);
            }

            return BadRequest(ModelState);
        }

        [HttpGet("CurrentStatistic")]
        public ActionResult CurrentStatistic()
        {
            var result = new List<SourceCurrentStatisticModel>();
            
            foreach (Source source in DatabaseContext.Sources)
            {
                var logs = DatabaseContext.Logs.Where(x =>
                    x.Time >= DateTime.UtcNow.Add(SettingsService.Get<TimeSpan>("SourceLogCountTime")) &&
                    x.SourceId == source.SourceId);

                var levels = logs.GroupBy(x => x.LogLevel).Select(x =>
                    new KeyValuePair<LogLevel, int>(
                    
                        x.Key,
                        x.Sum(d => 1)
                    )).ToList();
                
                LogLevelExtension.Levels().Where(x => !levels.Any(d => d.Key == x)).ToList().ForEach(x =>
                {
                    levels.Add(new KeyValuePair<LogLevel, int>(x,0));
                });
                
                var typs = logs.GroupBy(x => x.LogTyp).Select(x =>
                    new KeyValuePair<LogTyp, int>(
                    
                        x.Key,
                        x.Sum(d => 1)
                    )).ToList();
                
                LogTypExtension.Typs.Where(x => !typs.Any(d => d.Key == x)).ToList().ForEach(x =>
                {
                    typs.Add(new KeyValuePair<LogTyp, int>(x,0));
                });
                
                SourceCurrentStatisticModel tmp = new SourceCurrentStatisticModel()
                {
                    Source = source,
                    Count = logs.Sum(x => 1),
                    AllCount = DatabaseContext.Logs.Where(x => x.SourceId == source.SourceId).Sum(x => 1),
                    Level = levels.OrderBy(x => x.Key).ToList(),
                    Typ = typs.OrderBy(x => x.Key).ToList()
                };
                result.Add(tmp);
                
            }

            return Ok(result);
        }
    }

   
}