using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace SpecsFor.Mvc
{
	public class BrowserDriver
	{
		private readonly Func<IWebDriver> _browserFactory;

		public static readonly BrowserDriver InternetExplorer;
		public static readonly BrowserDriver Firefox;
		public static readonly BrowserDriver Chrome;

		static BrowserDriver()
		{
			InternetExplorer = new BrowserDriver(() =>
				{
					var options = new InternetExplorerOptions {IntroduceInstabilityByIgnoringProtectedModeSettings = true};

					return new InternetExplorerDriver(options);
				});

			Firefox = new BrowserDriver(() =>
				{
					var capabilities = new DesiredCapabilities();

					return new FirefoxDriver(capabilities);
				});

			Chrome = new BrowserDriver(() =>
				{
					return new ChromeDriver();
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
