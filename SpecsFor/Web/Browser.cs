using System;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

namespace SpecsFor.Web
{
	public class Browser
	{
		private readonly Func<IWebDriver> _browserFactory;
		public static readonly Browser InternetExplorer;

		static Browser()
		{
			InternetExplorer = new Browser(() =>
			                               	{
			                               		var capabilities = new DesiredCapabilities();
			                               		capabilities.SetCapability(InternetExplorerDriver.IntroduceInstabilityByIgnoringProtectedModeSettings, true);

			                               		return new InternetExplorerDriver(capabilities);
			                               	});
		}

		private Browser(Func<IWebDriver> browserFactory)
		{
			_browserFactory = browserFactory;
		}

		public IWebDriver Factory()
		{
			return _browserFactory();
		}
	}
}