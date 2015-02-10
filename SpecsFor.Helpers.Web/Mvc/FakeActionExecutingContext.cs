using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace SpecsFor.Helpers.Web.Mvc
{
	public class FakeActionExecutingContext : ActionExecutingContext
	{
		public FakeActionExecutingContext()
			//TODO: Wire up mock objects and pass them along!
			: base(new ControllerContext(new FakeHttpContext(), new RouteData(), new Mock<ControllerBase>().Object),
			new ReflectedActionDescriptor(typeof(ControllerBase).GetMethods()[0], "Test", new ReflectedControllerDescriptor(typeof(ControllerBase))),
			new Dictionary<string, object>())
		{


		}
	}
}