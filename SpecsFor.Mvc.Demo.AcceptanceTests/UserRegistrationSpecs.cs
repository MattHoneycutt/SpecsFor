using NUnit.Framework;
using Should;
using SpecsFor.Mvc.Demo.Controllers;
using SpecsFor.Mvc.Demo.Models;
using System.Linq;
using SpecsFor.Mvc.Helpers;
using SpecsFor.Mvc.Smtp;

namespace SpecsFor.Mvc.Demo.AcceptanceTests
{
	public class UserRegistrationSpecs
	{
		public class when_a_new_user_registers_with_invalid_data : SpecsFor<MvcWebApp>
		{
			protected override void Given()
			{
				SUT.NavigateTo<AccountController>(c => c.Register());
			}

			protected override void When()
			{
				SUT.FindFormFor<RegisterModel>()
					.Field(m => m.Email).SetValueTo("notanemail")
					//.Field(m => m.UserName).SetValueTo("Test User") --Omit a required field.
					.Field(m => m.Password).SetValueTo("P@ssword!")
					.Field(m => m.ConfirmPassword).SetValueTo("SomethingElse")
					.Submit();
			}

			[Test]
			public void then_it_redisplays_the_form()
			{
				SUT.Route.ShouldMapTo<AccountController>(c => c.Register());
			}

			[Test]
			public void then_it_marks_the_username_field_as_invalid()
			{
				SUT.FindFormFor<RegisterModel>()
					.Field(m => m.UserName).ShouldBeInvalid();
			}

			[Test]
			public void then_it_marks_the_email_as_invalid()
			{
				SUT.FindFormFor<RegisterModel>()
					.Field(m => m.Email).ShouldBeInvalid();
			}
		}

		public class when_a_new_user_registers : SpecsFor<MvcWebApp>
		{
			protected override void Given()
			{
				SUT.NavigateTo<AccountController>(c => c.Register());
			}

			protected override void When()
			{
				SUT.FindFormFor<RegisterModel>()
					.Field(m => m.Email).SetValueTo("test@user.com")
					.Field(m => m.UserName).SetValueTo("Test User")
					.Field(m => m.Password).SetValueTo("P@ssword!")
					.Field(m => m.ConfirmPassword).SetValueTo("P@ssword!")
					.Submit();
			}

			[Test]
			public void then_it_redirects_to_the_home_page()
			{
				SUT.Route.ShouldMapTo<HomeController>(c => c.Index());
			}

			[Test]
			public void then_it_sends_the_user_an_email()
			{
				SUT.Mailbox().MailMessages.Count().ShouldEqual(1);
			}

			[Test]
			public void then_it_sends_to_the_right_address()
			{
				SUT.Mailbox().MailMessages[0].To[0].Address.ShouldEqual("test@user.com");
			}

			[Test]
			public void then_it_comes_from_the_expected_address()
			{
				SUT.Mailbox().MailMessages[0].From.Address.ShouldEqual("registration@specsfor.com");
			}
		}
	}
}