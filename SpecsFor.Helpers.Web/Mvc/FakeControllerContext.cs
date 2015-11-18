using System;
using System.Collections.Specialized;
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

		public static void FakeAjaxRequest<TController>(this ISpecs<TController> specs)
			where TController : Controller
		{
			if (!(specs.SUT.ControllerContext is FakeControllerContext))
			{
				throw new NotSupportedException("The FakeAjaxRequest extension method can only be used if the current ControllerContext is of type FakeControllerContext.");	
			}

			specs.GetMockFor<IHeadersParamsProvider>().Setup(x => x.Values).Returns(
				new NameValueCollection()
				{
					{"X-Requested-With", "XMLHttpRequest"}
				});
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