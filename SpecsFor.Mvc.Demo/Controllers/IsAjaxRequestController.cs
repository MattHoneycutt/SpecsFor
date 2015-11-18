using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpecsFor.Mvc.Demo.Controllers
{
    public class IsAjaxRequestController : Controller
    {
        // GET: IsAjaxRequest
        public ActionResult Index()
        {
            return View();
        }

		[HttpPost]
		public ActionResult Index(string name)
		{
			string message = string.Format("Hi {0}!", name);

			if (Request.IsAjaxRequest())
			{
				return Json(message);
			}

			ViewBag.Message = message;
			return View();
		}
	}
}