using System;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

namespace SpecsFor.Mvc
{
	public class BrowserDriver
	{
		private readonly Func<IWebDriver> _browserFactory;

		public static readonly BrowserDriver InternetExplorer;

		static BrowserDriver()
		{
			InternetExplorer = new BrowserDriver(() =>
			                               	{
			                               		var capabilities = new DesiredCapabilities();
			                               		capabilities.SetCapability(InternetExplorerDriver.IntroduceInstabilityByIgnoringProtectedModeSettings, true);

			                               		return new InternetExplorerDriver(capabilities);
			                               	});
		}

		private BrowserDriver(Func<IWebDriver> browserFactory)
		{
			_browserFactory = browserFactory;
		}

		public IWebDriver CreateDriver()
		{
			return _browserFactory();
		}
	}
}