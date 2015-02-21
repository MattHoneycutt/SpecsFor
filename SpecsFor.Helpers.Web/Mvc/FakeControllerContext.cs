using System.Web.Mvc;
using System.Web.Routing;

namespace SpecsFor.Helpers.Web.Mvc
{
	public static class FakeControllerContextExtensions
	{
		public static void UseFakeContextForController<TController>(this ISpecs<TController> specs)
			where TController : Controller
		{
			specs.SUT.ControllerContext = specs.MockContainer.GetInstance<FakeControllerContext>();
		}
	}

	public class FakeControllerContext : ControllerContext
	{
		public FakeControllerContext(FakeHttpContext context, RouteData routeData, ControllerBase controllerBase) 
			: base(context, routeData, controllerBase)
		{
			
		}
	}
}