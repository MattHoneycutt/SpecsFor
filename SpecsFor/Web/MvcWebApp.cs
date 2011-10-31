using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.TestHelper.Fakes;
using WatiN.Core;
using Microsoft.Web.Mvc;
using MvcContrib.TestHelper;

namespace SpecsFor.Web
{
	public class MvcWebApp : IE
	{
		public static string BaseUrl = "http://localhost";

		public FormHelper<T> FindFormFor<T>()
		{
			return new FormHelper<T>(this);
		}

		public Element ValidationSummary
		{ 
			get
			{
				return Element(Find.ByClass("validation-summary-errors"));
			}
		}

		public RouteData Route
		{
			get
			{
				//Strip the host, port, etc. off the route.
				var url = this.Url.Replace(BaseUrl, "~");

				return url.Route();
			}
		}

		private class FakeViewDataContainer : IViewDataContainer
		{
			private ViewDataDictionary _viewData = new ViewDataDictionary();
			public ViewDataDictionary ViewData { get { return _viewData; } set { _viewData = value; } }
		}

		public void NavigateTo<TController>(Expression<Action<TController>> action) where TController : Controller
		{
			var helper = new HtmlHelper(new ViewContext { HttpContext = FakeHttpContext.Root() }, new FakeViewDataContainer());

			var url = helper.BuildUrlFromExpression(action);

			GoTo(BaseUrl + url);
		}
	}

	public class FormHelper<T>
	{
		private readonly MvcWebApp _webApp;

		public FormHelper(MvcWebApp webApp)
		{
			_webApp = webApp;
		}

		public FormHelper<T> SetFieldValue(Expression<Func<T, object>> property, string value)
		{
			var name = ExpressionHelper.GetExpressionText(property);

			var field = _webApp.TextField(Find.ByName(name));

			field.TypeText(value);

			return this;
		}

		public void Submit()
		{
			//TODO: Probably not the best way to find a form...
			_webApp.Forms[0].Submit();
		}
	}
}