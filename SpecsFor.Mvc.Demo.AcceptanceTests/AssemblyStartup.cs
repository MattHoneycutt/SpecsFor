using NUnit.Framework;

namespace SpecsFor.Mvc.Demo.AcceptanceTests
{
	[SetUpFixture]
	public class AssemblyStartup
	{
		private SpecsForIntegrationHost _host;

		[SetUp]
		public void SetupTestRun()
		{
			var config = new SpecsForMvcConfig();
			config.UseIISExpress()
				.With(Project.Named("SpecsFor.Mvc.Demo"))
				.ApplyWebConfigTransformForConfig("Debug");

			config.BuildRoutesUsing(r => MvcApplication.RegisterRoutes(r));
			config.UseBrowser(BrowserDriver.InternetExplorer);

			_host = new SpecsForIntegrationHost(config);
			_host.Start();
		}

		[TearDown]
		public void TearDownTestRun()
		{
			_host.Shutdown();
		}
	}
}