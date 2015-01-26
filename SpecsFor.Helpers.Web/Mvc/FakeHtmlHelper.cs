using System.Web.Mvc;

namespace SpecsFor.Helpers.Web.Mvc
{
	public class FakeHtmlHelper : HtmlHelper
	{
		public FakeHtmlHelper(FakeViewContext viewContext, FakeViewDataContainer viewDataContainer = null)
			: base(viewContext ?? new FakeViewContext(), viewDataContainer ?? new FakeViewDataContainer())
		{

		}
	}

	public class FakeHtmlHelper<TModel> : HtmlHelper<TModel> where TModel : class
	{
		public FakeHtmlHelper(TModel model, FakeViewContext viewContext = null)
			: base(viewContext ?? new FakeViewContext(), new FakeViewDataContainer<TModel>(model))
		{

		}
	}
}