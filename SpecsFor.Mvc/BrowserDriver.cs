using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace SpecsFor.Mvc
{
	//TODO: Split this so that the implementation and DSL aren't sharing classes. 
	public class BrowserDriver
	{
		private readonly Func<RemoteWebDriver> _browserFactory;

		public static readonly BrowserDriver InternetExplorer;
		public static readonly BrowserDriver Firefox;
		public static readonly BrowserDriver Chrome;
		private RemoteWebDriver _driver;

		static BrowserDriver()
		{
			InternetExplorer = new BrowserDriver(() =>
				{
					var options = new InternetExplorerOptions { IntroduceInstabilityByIgnoringProtectedModeSettings = true };

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

		public BrowserDriver(Func<RemoteWebDriver> browserFactory)
		{
			_browserFactory = browserFactory;
		}

		public RemoteWebDriver GetDriver()
		{
			try
			{
				return _driver ?? (_driver = _browserFactory());
			}
			catch (DriverServiceNotFoundException ex)
			{
				throw new DriverNotFoundException(
					"The configured web driver could not be initialized because the driver executable was not found in '" +
					Directory.GetCurrentDirectory() +
					"'.  Make sure the driver is copied to the output directory of your spec project, or install the " +
					"driver in a location, and add that location to your PATH environment variable.", ex);
			}
		}

		public void Shutdown()
		{
			if (_driver != null)
			{
				_driver.Quit();
				_driver = null;
			}
		}
	}
}
