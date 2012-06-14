using System;
using System.Web.Mvc;
using SpecsFor.Mvc.Demo.Models;

namespace SpecsFor.Mvc.Demo.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.Message = "Welcome to ASP.NET MVC!";

			return View();
		}

		[Authorize]
		public ActionResult About()
		{
			return View(new AboutViewModel
			            	{
			            		DayOfWeek = DateTime.Today.DayOfWeek.ToString(),
								User = new UserViewModel
								       	{
								       		UserName = string.IsNullOrEmpty(HttpContext.User.Identity.Name) ? "Anonymous" : HttpContext.User.Identity.Name
								       	},
								BusinessDays = new[] { "Monday", "Wednesday", "Friday", "Saturday"}
			            	});
		}
	}
}
