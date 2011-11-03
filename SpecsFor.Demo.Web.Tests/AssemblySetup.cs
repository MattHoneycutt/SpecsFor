using System.Web.Routing;
using NUnit.Framework;
using OpenQA.Selenium.IE;
using SpecsFor.Web;

namespace SpecsFor.Demo.Web.UITests
{
	[SetUpFixture]
	public class AssemblySetup
	{
		[SetUp]
		public void SetUpUITests()
		{
			MvcWebApp.BaseUrl = "http://localhost:52125";
			MvcApplication.RegisterRoutes(RouteTable.Routes);

			//TODO: Open questions to be answered:
			//1) How do we determine where the app is running?
			//2) How do we point the app at a database for testing? 
		}
	}
}