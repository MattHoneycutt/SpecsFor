using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace SpecsFor.Mvc
{
	public static class MvcWebAppAssertionExtensions
	{
		public static void UrlShouldMapTo<TController>(this MvcWebApp app, Expression<Action<TController>> action) where TController : Controller
		{
			var helper = new FakeHtmlHelper(new FakeViewContext());
			var expectedUrl = MvcWebApp.BaseUrl + helper.BuildUrlFromExpression(action);
			app.Browser.Url.ShouldEqual(expectedUrl);
		}
	}
}