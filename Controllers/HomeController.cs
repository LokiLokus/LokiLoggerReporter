using System;
using System.Diagnostics;
using System.Linq;
using lokiloggerreporter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using static System.String;

namespace lokiloggerreporter.Controllers {
	[Route("/")]
	public class HomeController : Controller {
		public DatabaseCtx DatabaseCtx { get; set; }
		public HomeController(DatabaseCtx databaseCtx)
		{
			DatabaseCtx = databaseCtx;
		}
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet("Source/{source}")]
		public IActionResult AnalyzeSource([FromRoute]string source)
		{
			return View("AnalyzeSource");
		}

		[HttpGet("RestAnalyzer")]
		public IActionResult RestAnalyzer()
		{
			return View("RestAnalyzer");
		}
		
		[HttpGet("AnalyzeSource")]
		public IActionResult AnalyzeSource()
		{
			return View("AnalyzeSource");
		}

		[HttpGet("Table")]
		public IActionResult Table()
		{
			return View("Table");
		}


		[HttpGet("Source")]
		public IActionResult Source()
		{
			return View("Source");
		}
		
		[HttpGet("Table/{source}")]
		public IActionResult Table([FromRoute]string source)
		{
			return View("Table");
		}
	}
}