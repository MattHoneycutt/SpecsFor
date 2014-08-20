using System.Web.Mvc;

namespace SpecsFor.Helpers.Web.Mvc
{
	public class FakeUrlHelper : UrlHelper
	{
		public FakeUrlHelper(FakeRequestContext context)
			: base(context ?? new FakeRequestContext())
		{
		}
	}
}