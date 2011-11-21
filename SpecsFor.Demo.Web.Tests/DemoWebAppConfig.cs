using NUnit.Framework;
using SpecsFor.Web;

namespace SpecsFor.Demo.Web.UITests
{
	[SetUpFixture]
	public class DemoWebAppConfig : SpecsForMvcConfig
	{
		public DemoWebAppConfig()
		{
			UseIISExpressWith(Project("SpecsFor.Demo.Web"));
			BuildRoutesUsing(r => MvcApplication.RegisterRoutes(r));
			UseBrowser(Browser.InternetExplorer);

			//TODO: Open questions to be answered:
			//1) How do we point the app at a database for testing?
		}

		[SetUp]
		public override void SetupTestRun()
		{
			base.SetupTestRun();
		}

		[TearDown]
		public override void TearDownTestRun()
		{
			base.TearDownTestRun();
		}
	}
}