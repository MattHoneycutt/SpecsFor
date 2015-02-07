using System.Web.Mvc;
using Microsoft.Web.Mvc;
using SpecsFor.Mvc.Demo.Areas.Tasks.Models;

namespace SpecsFor.Mvc.Demo.Areas.Tasks.Controllers
{
	[ActionLinkArea("Tasks")]
    public class ListController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(int newId, string name)
        {
            return null;
        }

		public ActionResult Create()
		{
			return View(new Task());
		}

        [HttpPost]
        public ActionResult Create(Task model)
        {
            if (model.Complete)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Create");
        }
    }
}
