using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.TestHelper;
using MvcContrib.TestHelper.Fakes;
using OpenQA.Selenium;
using Microsoft.Web.Mvc;

namespace SpecsFor.Mvc
{
	public class MvcWebApp : IDisposable
	{
		public static string BaseUrl { get; set; }
		public static BrowserDriver Driver { get; set; }

		private bool _hasQuit;

		public IWebDriver Browser { get; private set; }

		static MvcWebApp()
		{
			BaseUrl = "http://localhost";
			Driver = BrowserDriver.InternetExplorer;
		}

		public MvcWebApp()
		{
			Browser = Driver.CreateDriver();
		}

		public FormHelper<T> FindFormFor<T>()
		{
			return new FormHelper<T>(this);
		}

		public IWebElement ValidationSummary
		{ 
			get
			{
				return Browser.FindElement(By.ClassName("validation-summary-errors"));
			}
		}

		public RouteData Route
		{
			get
			{
				//Strip the host, port, etc. off the route.  The routing helpers
				//expect the URL to look like "~/virtual/path"
				var url = Browser.Url.Replace(BaseUrl, "~");

				return url.Route();
			}
		}

		public void NavigateTo<TController>(Expression<Action<TController>> action) where TController : Controller
		{
			var helper = new HtmlHelper(new ViewContext { HttpContext = FakeHttpContext.Root() }, new FakeViewDataContainer());

			var url = helper.BuildUrlFromExpression(action);

			Browser.Navigate().GoToUrl(BaseUrl + url);
		}

		public void Dispose()
		{
			//Not all of the web drivers have implemented IDisposable correctly.  Some will dispose
			//but won't actually exit.  This wrapper fixes that inconsistent behavior. 
			if (!_hasQuit)
			{
				_hasQuit = true;
				Browser.Quit();
			}

			Browser.Dispose();
		}
	}
}