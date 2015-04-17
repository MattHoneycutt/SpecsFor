using System.Security.Principal;
using WindowsAuthSampleApp.Controllers;
using WindowsAuthSampleApp.Models;
using NUnit.Framework;
using Should;
using SpecsFor;
using SpecsFor.Mvc;

namespace WindowsAuthSampleApp.Specs.Controllers
{
	public class HomeControllerSpecs
	{
		public class when_viewing_the_home_page : SpecsFor<MvcWebApp>
		{
			protected override void When()
			{
				SUT.NavigateTo<HomeController>(c => c.Index());
			}

			[Test]
			public void then_it_has_the_name_of_the_logged_in_user()
			{
				SUT.FindDisplayFor<HomePageViewModel>()
					.DisplayFor(x => x.UserName).Text.ShouldEqual(WindowsIdentity.GetCurrent().Name);
			}
		}
	}
}