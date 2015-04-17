using System.Web.Mvc;
using WindowsAuthSampleApp.Models;

namespace WindowsAuthSampleApp.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View(new HomePageViewModel
			{
				UserName = User.Identity.Name
			});
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}