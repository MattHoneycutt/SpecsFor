using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.TestHelper;
using MvcContrib.TestHelper.Fakes;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using Microsoft.Web.Mvc;
using OpenQA.Selenium.Remote;

namespace SpecsFor.Web
{
	//TODO: Refactor this so it doesn't inherit directly from IE, but instead
	//		can work with any browser. 
	public class MvcWebApp : InternetExplorerDriver
	{
		public static string BaseUrl = "http://localhost";

		public MvcWebApp() : base(GetCapabilities())
		{
			
		}

		private static DesiredCapabilities GetCapabilities()
		{
			//This hackiness is needed to work around the way IE is configured at FIS.  I've not encountered any problems *yet*, but 
			//we might in the future.  Who knows... 
			var capabilities = new DesiredCapabilities();
			capabilities.SetCapability(InternetExplorerDriver.IntroduceInstabilityByIgnoringProtectedModeSettings, true);

			return capabilities;
		}

		public FormHelper<T> FindFormFor<T>()
		{
			return new FormHelper<T>(this);
		}

		public IWebElement ValidationSummary
		{ 
			get
			{
				return FindElement(By.ClassName("validation-summary-errors"));
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

			Navigate().GoToUrl(BaseUrl + url);
		}

		private bool HasQuit;

		protected override void Dispose(bool disposing)
		{
			if (!HasQuit)
			{
				HasQuit = true;
				Quit();
			}

			base.Dispose(disposing);
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

			var field = _webApp.FindElement(By.Name(name));

			field.SendKeys(value);

			return this;
		}

		public void Submit()
		{
			//TODO: Probably not the best way to find a form...
			_webApp.FindElement(By.TagName("form")).Submit();
		}
	}
}