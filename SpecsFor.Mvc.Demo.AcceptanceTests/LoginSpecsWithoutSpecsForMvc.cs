using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using Should;

namespace SpecsFor.Mvc.Demo.AcceptanceTests
{
	[TestFixture]
	[Explicit("These tests exist for illustration purposes only.")]
	public class LoginSpecsWithoutSpecsForMvc
	{
		[Test]
		public void when_logging_in_with_an_invalid_username_and_password()
		{
			var options = new InternetExplorerOptions {IntroduceInstabilityByIgnoringProtectedModeSettings = true};
			var driver = new InternetExplorerDriver(options);

			try
			{
				driver.Navigate().GoToUrl("http://localhost:52125/Account/LogOn");

				driver.FindElement(By.Name("UserName")).SendKeys("bad@user.com");
				driver.FindElement(By.Name("Password")).SendKeys("BadPass");
				driver.FindElement(By.TagName("form")).Submit();

				driver.Url.ShouldEqual("http://localhost:52125/Account/LogOn");

				driver.FindElement(By.ClassName("validation-summary-errors")).Text.ShouldContain(
					"The user name or password provided is incorrect.");
			}
			finally
			{
				driver.Close();
			}
		}

		[Test]
		public void when_logging_in_with_a_valid_username_and_password()
		{
			var options = new InternetExplorerOptions { IntroduceInstabilityByIgnoringProtectedModeSettings = true };
			var driver = new InternetExplorerDriver(options);

			try
			{
				driver.Navigate().GoToUrl("http://localhost:52125/Account/LogOn");

				driver.FindElement(By.Name("UserName")).SendKeys("real@user.com");
				driver.FindElement(By.Name("Password")).SendKeys("RealPassword");
				driver.FindElement(By.TagName("form")).Submit();

				driver.Url.ShouldEqual("http://localhost:52125/");
			}
			finally
			{
				driver.Close();
			}
		}
	}
}