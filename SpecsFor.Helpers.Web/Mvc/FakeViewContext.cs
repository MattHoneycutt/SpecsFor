using System.Web.Mvc;

namespace SpecsFor.Helpers.Web.Mvc
{
	public class FakeViewContext : ViewContext
	{
		public FakeViewContext()
		{
			HttpContext = new FakeHttpContext();
		}
	}
}