using System.Collections.Generic;
using System.Linq;
using lokiloggerreporter.Models;
using Microsoft.AspNetCore.Mvc;

namespace lokiloggerreporter.Rest {
	[Route("api/Source")]
	[ApiController]
	public class SourceRest :Controller{
		public DatabaseCtx ctx { get; set; }
		public SourceRest(DatabaseCtx dctx)
		{
			ctx = dctx;
		}


		[HttpGet("All")]
		public ActionResult GetAllSources()
		{

			var data = ctx.Logs.GroupBy(x => x.Name.ToLower())
				.Select(x => new
				{
					Source = x.Key,
					Count = x.Count(),
					Level = x.GroupBy(z => z.LogLevel).Select(z =>
					new {
						z.Key,
						Count = z.Count()
					}),
					Typ = x.GroupBy(z => z.LogTyp).Select(z =>
						new {
							z.Key,
							Count = z.Count()
						})
				});
			return Ok(data);
		}
	}
}