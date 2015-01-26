using System.Web.Mvc;

namespace SpecsFor.Helpers.Web.Mvc
{
	public class FakeViewDataContainer : IViewDataContainer
	{
		public ViewDataDictionary ViewData { get; set; }

		public FakeViewDataContainer()
		{
			ViewData = new ViewDataDictionary();
		}
	}

	public class FakeViewDataContainer<TModel> : IViewDataContainer
	{
		public ViewDataDictionary ViewData { get; set; }

		public FakeViewDataContainer(TModel model = default(TModel))
		{
			ViewData = new ViewDataDictionary<TModel>(model);
		}
	}
}