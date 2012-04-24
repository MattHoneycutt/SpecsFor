using System;
using NUnit.Framework;
using SpecsFor.Mvc.Demo.Controllers;
using Should;
using SpecsFor.Mvc.Demo.Models;

namespace SpecsFor.Mvc.Demo.AcceptanceTests
{
	public class AboutSpecs
	{
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
			public void then_it_knows_the_user_is_anonymous()
			{
				SUT.FindDisplayFor<AboutViewModel>()
					.DisplayFor(m => m.User.UserName).Text.ShouldEqual("Anonymous");
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