using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
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
	public class MvcWebApp
	{
		//TODO: Move to Service Locator class?  

		public static readonly IList<Action<MvcWebApp>> PreTestCallbacks = new List<Action<MvcWebApp>>();
		public static string BaseUrl { get; set; }
		public static BrowserDriver Driver { get; set; }
		public static IHandleAuthentication Authentication { get; set; }
		public static TimeSpan Delay { get; set; }

		public IWebDriver Browser { get; private set; }

		static MvcWebApp()
		{
			BaseUrl = "http://localhost";
			Driver = BrowserDriver.InternetExplorer;
		}

		public MvcWebApp()
		{
			Browser = Driver.GetDriver();

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

				var queryStringParams = new Dictionary<string, string>();

				//Parse out the query string params. 
				if (url.Contains("?"))
				{
					var parts = url.Split('?');
					url = parts[0];

					queryStringParams = parts[1].Split('&')
						.Select(v => v.Split('='))
						.ToDictionary(a => a[0], a => a[1]);
				}
				
				var context = new FakeHttpContext(url, null, null, null, null, null);
				context.SetRequest(new FakeHttpRequest(url, new Uri(Browser.Url), null));

				var routeData = RouteTable.Routes.GetRouteData(context);

				//Add in query string params.  This will allow the ShouldMapTo extension method to work 
				//with query string parameters.
				foreach (var kvp in queryStringParams.Where(kvp => !routeData.Values.ContainsKey(kvp.Key)))
				{
					routeData.Values.Add(kvp.Key, kvp.Value);
				}

				return routeData;
			}
		}

		public void NavigateTo<TController>(Expression<Action<TController>> action) where TController : Controller
		{
			var helper = new HtmlHelper(new ViewContext { HttpContext = FakeHttpContext.Root() }, new FakeViewDataContainer());

			var url = helper.BuildUrlFromExpression(action);

			Browser.Navigate().GoToUrl(BaseUrl + url);
		}

		public IWebElement FindLinkTo<TController>(Expression<Action<TController>> action) where TController : Controller
		{
			var helper = new HtmlHelper(new ViewContext { HttpContext = FakeHttpContext.Root() }, new FakeViewDataContainer());

			var url = helper.BuildUrlFromExpression(action);

			var element = Browser.FindElement(By.CssSelector("a[href='" + url + "']"));

			return element;
		}

		internal void Pause()
		{
			if (Delay != default(TimeSpan))
			{
				Thread.Sleep(Delay);
			}
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

		public string AllText()
		{
			return Browser.FindElement(By.TagName("body")).Text;
		}
	}
}