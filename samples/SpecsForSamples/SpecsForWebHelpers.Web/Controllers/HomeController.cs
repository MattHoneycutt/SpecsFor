using System.Web.Mvc;
using Microsoft.Web.Mvc;
using SpecsForWebHelpers.Web.Domain;
using SpecsForWebHelpers.Web.Models;

namespace SpecsForWebHelpers.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ICurrentUser _currentUser;

		public HomeController(ICurrentUser currentUser)
		{
			_currentUser = currentUser;
		}

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult SetName()
		{
			return View();
		}

		[HttpPost]
		public ActionResult SetName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				ViewBag.Error = "You must specify a name!";
				return View();
			}

			_currentUser.SetName(name);

			return RedirectToAction("Index", "Home");
		}

		public ActionResult SayHello(string name)
		{
			var model = new SayHelloViewModel
			{
				Name = name
			};

			return View(model);
		}

		[HttpPost]
		public ActionResult SayHello(SayHelloForm form)
		{
			return this.RedirectToAction(c => c.SayHello(form.Name));
		}
	}
}