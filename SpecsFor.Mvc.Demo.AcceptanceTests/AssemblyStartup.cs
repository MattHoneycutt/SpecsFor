using NUnit.Framework;
using SpecsFor.Mvc.Demo.Areas.Tasks;

namespace SpecsFor.Mvc.Demo.AcceptanceTests
{
	[SetUpFixture]
	public class AssemblyStartup
	{
		private SpecsForIntegrationHost _host;

		[OneTimeSetUp]
		public void SetupTestRun()
		{
			var config = new SpecsForMvcConfig();
			config.UseIISExpress()
				.UsePort(44300)
				.UseHttps()
				.With(Project.Named("SpecsFor.Mvc.Demo"))
				.CleanupPublishedFiles()
				.ApplyWebConfigTransformForConfig("Test");

			//TODO: The order of registration matters right now, but it shouldn't. 
			config.RegisterArea<TasksAreaRegistration>();
			config.BuildRoutesUsing(r => MvcApplication.RegisterRoutes(r));

			//NOTE: You can use whatever browser you want.  For build servers, you can check an environment
			//		variable to determine which browser to use, enabling you to re-run the same suite of
			//		tests once for each browser. 
			//config.UseBrowser(BrowserDriver.InternetExplorer);
			config.UseBrowser(BrowserDriver.Chrome);
			//config.UseBrowser(BrowserDriver.Firefox);

			config.InterceptEmailMessagesOnPort(13565);

			config.AuthenticateBeforeEachTestUsing<StandardAuthenticator>();

		    config.SetBrowserWindowSize(width: 1024, height: 768);

			_host = new SpecsForIntegrationHost(config);
			_host.Start();
		}

		[OneTimeTearDown]
		public void TearDownTestRun() 
		{
			_host.Shutdown();
		}
	}
}