using System;
using System.Diagnostics;
using System.Linq;
using lokiloggerreporter.Models;
using Microsoft.AspNetCore.Mvc;
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

		[HttpGet("Source")]
		public IActionResult AnalyzeSource()
		{
			return View("AnalyzeSource");
		}
	}
}