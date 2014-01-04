using NUnit.Framework;
using Should;
using SpecsFor.Mvc.Demo.Controllers;
using SpecsFor.Mvc.Demo.Models;
using SpecsFor.Mvc.Helpers;

namespace SpecsFor.Mvc.Demo.AcceptanceTests
{
	public class LoginSpecs
	{
		public class when_logging_in_with_an_invalid_username_and_password : SpecsFor<MvcWebApp>
		{
			protected override void Given()
			{
				//Make sure we're already logged out
				SUT.NavigateTo<AccountController>(c => c.LogOff());

				SUT.NavigateTo<AccountController>(c => c.LogOn());
			}

			protected override void When()
			{
				SUT.FindFormFor<LogOnModel>()
					.Field(m => m.UserName).SetValueTo("bad@user.com")
					.Field(m => m.Password).SetValueTo("BadPass")
					.Submit();
			}

			[Test]
			public void then_it_should_redisplay_the_page()
			{
				SUT.Route.ShouldMapTo<AccountController>(c => c.LogOn());
			}

			[Test]
			public void then_it_should_contain_a_validation_error()
			{
				SUT.ValidationSummary.Text.ShouldContain("The user name or password provided is incorrect.");
			}
		}

		public class when_logging_in_with_valid_credentials : SpecsFor<MvcWebApp>
		{
			protected override void Given()
			{
				SUT.NavigateTo<AccountController>(c => c.LogOn());
			}

			protected override void When()
			{
				SUT.FindFormFor<LogOnModel>()
					.Field(m => m.UserName).SetValueTo("real@user.com")
					.Field(m => m.Password).SetValueTo("RealPassword")
					.Submit();
			}

			[Test]
			public void then_it_redirects_to_the_home_page()
			{
				SUT.Route.ShouldMapTo<HomeController>(c => c.Index());
			}
		}
	}
}