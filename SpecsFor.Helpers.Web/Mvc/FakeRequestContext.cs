using System.Web.Routing;

namespace SpecsFor.Helpers.Web.Mvc
{
	public class FakeRequestContext : RequestContext
	{
		public FakeRequestContext(FakeHttpContext httpContext = null)
			: base(httpContext ?? new FakeHttpContext(), new RouteData())
		{
		}
	}
}