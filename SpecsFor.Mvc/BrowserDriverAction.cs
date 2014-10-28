namespace SpecsFor.Mvc
{
	public class BrowserDriverAction : ITestRunnerAction
	{
		private readonly BrowserDriver _driver;

		public BrowserDriverAction(BrowserDriver driver)
		{
			_driver = driver;
		}

		public void Startup()
		{
			MvcWebApp.Driver = _driver;
		}

		public void Shutdown()
		{
			_driver.Shutdown();
		}
	}
}