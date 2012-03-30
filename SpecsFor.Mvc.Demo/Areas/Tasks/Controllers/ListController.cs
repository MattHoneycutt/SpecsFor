using System.Web.Mvc;
using Microsoft.Web.Mvc;

namespace SpecsFor.Mvc.Demo.Areas.Tasks.Controllers
{
	[ActionLinkArea("Tasks")]
    public class ListController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
