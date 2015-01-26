using System.Web.Mvc;

namespace SpecsFor.Helpers.Web.Mvc
{
	public class FakeViewContext : ViewContext
	{
		public FakeViewContext(FakeHttpContext context = null)
		{
			HttpContext = context ?? new FakeHttpContext();
			ViewData = new ViewDataDictionary();
		}
	}
}