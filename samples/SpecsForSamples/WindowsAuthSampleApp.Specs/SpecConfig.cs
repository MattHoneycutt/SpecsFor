using NUnit.Framework;
using SpecsFor.Mvc;

namespace WindowsAuthSampleApp.Specs
{
	[SetUpFixture]
	public class SpecConfig
	{
		private SpecsForIntegrationHost _host;

		[SetUp]
		public void Setup()
		{
			var config = new SpecsForMvcConfig();
			config.UseIISExpress()
				.ApplicationHostConfigurationFile(@"applicationhost.config")
				.With(Project.Named("WindowsAuthSampleApp"));

			config.UseBrowser(BrowserDriver.InternetExplorer);

			_host = new SpecsForIntegrationHost(config);			
			_host.Start();
		}

		[TearDown]
		public void TearDown()
		{
			_host.Shutdown();
		}
	}
}