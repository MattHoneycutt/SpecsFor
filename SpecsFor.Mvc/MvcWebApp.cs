using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.TestHelper;
using MvcContrib.TestHelper.Fakes;
using OpenQA.Selenium;
using Microsoft.Web.Mvc;
using SpecsFor.Mvc.Authentication;
using SpecsFor.Mvc.Helpers;
using System.Linq;

namespace SpecsFor.Mvc
{
	//NOTE: MvcWebApp has definitely picked up too many responsibilites.  It's in need of 
	//		refactoring.  The project could probably benefit from a simple IoC container 
	//		or service locator for handling some of these things. 
	public class MvcWebApp : IDisposable
	{
		//TODO: Move to Service Locator class?  

		public static readonly IList<Action<MvcWebApp>> PreTestCallbacks = new List<Action<MvcWebApp>>();
		public static string BaseUrl { get; set; }
		public static BrowserDriver Driver { get; set; }
		public static IHandleAuthentication Authentication { get; set; }
		public static TimeSpan Delay { get; set; }

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

			try
			{
				foreach (var callback in PreTestCallbacks)
				{
					callback(this);
				}

				if (Authentication != null)
				{
					Authentication.Authenticate(this);
				}
			}
			//If something happens and the class can't be created, we still need to destroy the browser.
			catch (Exception)
			{
				Browser.Quit();
				Browser.Dispose();
				throw;
			}
		}

		public FluentForm<T> FindFormFor<T>()
		{
			return new FluentForm<T>(this);
		}

		public FluentDisplay<T> FindDisplayFor<T>()
		{
			return new FluentDisplay<T>(this);
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

				var queryStringParams = new NameValueCollection();

				//Parse out the query string params. 
				if (url.Contains("?"))
				{
					var parts = url.Split('?');
					url = parts[0];

					foreach (var pair in parts[1].Split('&')
						.Select(v => v.Split('='))
						.Select(a => new KeyValuePair<string, string>(a[0], a[1])))
					{
						queryStringParams.Add(pair.Key, pair.Value);
					}
				}
				
				var context = new FakeHttpContext(url, null, null, queryStringParams, null, null);

				return RouteTable.Routes.GetRouteData(context);
			}
		}

		public void NavigateTo<TController>(Expression<Action<TController>> action) where TController : Controller
		{
			var helper = new HtmlHelper(new ViewContext { HttpContext = FakeHttpContext.Root() }, new FakeViewDataContainer());

			var url = helper.BuildUrlFromExpression(action);

			Browser.Navigate().GoToUrl(BaseUrl + url);
		}
		
		internal void Pause()
		{
			//Not all of the web drivers have implemented IDisposable correctly.  Some will dispose
			//but won't actually exit.  This wrapper fixes that inconsistent behavior. 
			if (!_hasQuit)
				if (Delay != default(TimeSpan))
				{
					_hasQuit = true;
					Browser.Close();
					Thread.Sleep(Delay);
				}
		}

		public void Dispose()
		{
			//Not all of the web drivers have implemented IDisposable correctly.  Some will dispose
			//but won't actually exit.  This wrapper fixes that inconsistent behavior. 
			if (!_hasQuit)
			{
				_hasQuit = true;
				Browser.Close();
			}

			Browser.Dispose();
		}

		public static void AddPreTestCallback(Action action)
		{
			AddPreTestCallback(_ => action());
		}

		public static void AddPreTestCallback(Action<MvcWebApp> action)
		{
			PreTestCallbacks.Add(action);
		}

		public IWebElement FindElementByExpression<T, TProp>(Expression<Func<T, TProp>> property)
		{
			var name = ExpressionHelper.GetExpressionText(property);
			name = TagBuilder.CreateSanitizedId(name);

			var field = Browser.FindElement(By.Id(name));
			return field;
		}
	}
}