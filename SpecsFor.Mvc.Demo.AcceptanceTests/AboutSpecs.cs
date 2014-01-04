using System;
using NUnit.Framework;
using SpecsFor.Mvc.Demo.Controllers;
using Should;
using SpecsFor.Mvc.Demo.Models;
using SpecsFor.Mvc.Helpers;

namespace SpecsFor.Mvc.Demo.AcceptanceTests
{
	public class AboutSpecs
	{
		public class when_viewing_the_about_page_without_being_authenticated : SpecsFor<MvcWebApp>
		{
			protected override void When()
			{
				SUT.NavigateTo<AccountController>(c => c.LogOff());
				SUT.NavigateTo<HomeController>(c => c.About());
			}

			[Test]
			public void then_it_should_redirect_to_the_login_controller()
			{
				SUT.Route.ShouldMapTo<AccountController>(c => c.LogOn());
			}
		}

		public class when_viewing_the_about_page : SpecsFor<MvcWebApp>
		{
			protected override void When()
			{
				SUT.NavigateTo<HomeController>(c => c.About());
			}

			[Test]
			public void then_it_displays_the_current_day_of_the_week()
			{
				SUT.FindDisplayFor<AboutViewModel>()
					.DisplayFor(m => m.DayOfWeek).Text.ShouldEqual(DateTime.Today.DayOfWeek.ToString());
			}

			[Test]
			public void then_it_knows_the_user_is_authenticated()
			{
				SUT.FindDisplayFor<AboutViewModel>()
					.DisplayFor(m => m.User.UserName).Text.ShouldEqual("real@user.com");
			}

			[Test]
			public void then_it_displays_the_correct_business_days()
			{
				SUT.FindDisplayFor<AboutViewModel>()
					.DisplayFor(m => m.BusinessDays[0]).Text.ShouldEqual("Monday");

				SUT.FindDisplayFor<AboutViewModel>()
					.DisplayFor(m => m.BusinessDays[3]).Text.ShouldEqual("Saturday");
			}
		}
	}
}