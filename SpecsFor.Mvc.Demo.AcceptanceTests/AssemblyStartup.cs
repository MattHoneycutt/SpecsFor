using NUnit.Framework;
using SpecsFor.Mvc.Demo.Areas.Tasks;

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
				.ApplyWebConfigTransformForConfig("Test");

			config.BuildRoutesUsing(r => MvcApplication.RegisterRoutes(r));
			config.RegisterArea<TasksAreaRegistration>();

			config.UseBrowser(BrowserDriver.InternetExplorer);

			config.InterceptEmailMessagesOnPort(13565);

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