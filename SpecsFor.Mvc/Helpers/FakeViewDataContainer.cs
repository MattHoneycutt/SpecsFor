using System.Web.Mvc;

namespace SpecsFor.Mvc.Helpers
{
	internal class FakeViewDataContainer : IViewDataContainer
	{
		public ViewDataDictionary ViewData { get; set; }

		public FakeViewDataContainer()
		{
			ViewData = new ViewDataDictionary();
		}
	}
}