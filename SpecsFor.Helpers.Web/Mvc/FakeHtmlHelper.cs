using System.Web.Mvc;
using Moq;

namespace SpecsFor.Helpers.Web.Mvc
{
	public class FakeHtmlHelper : HtmlHelper
	{
		public FakeHtmlHelper(IViewDataContainer viewDataContainer = null)
			: base(new FakeViewContext(), viewDataContainer ?? new Mock<IViewDataContainer>().Object)
		{

		}
	}

	public class FakeHtmlHelper<TModel> : HtmlHelper<TModel> where TModel : class
	{
		public FakeHtmlHelper(TModel model)
			: base(new FakeViewContext(), GetMockContainer(model))
		{

		}

		private static IViewDataContainer GetMockContainer(TModel model)
		{
			var dataContainer = new Mock<IViewDataContainer>();

			ViewDataDictionary<TModel> dataDictionary = new ViewDataDictionary<TModel>(model);

			dataContainer.Setup(c => c.ViewData).Returns(dataDictionary);

			return dataContainer.Object;
		}
	}
}