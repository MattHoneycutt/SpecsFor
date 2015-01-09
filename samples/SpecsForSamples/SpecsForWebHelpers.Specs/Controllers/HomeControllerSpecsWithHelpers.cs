using System.Web.Mvc;
using NUnit.Framework;
using SpecsFor;
using SpecsFor.Helpers.Web.Mvc;
using SpecsForWebHelpers.Web.Controllers;
using SpecsForWebHelpers.Web.Domain;
using SpecsForWebHelpers.Web.Models;

namespace SpecsForWebHelpers.Specs.Controllers
{
	public class HomeControllerSpecsWithHelpers
	{
		public class when_setting_the_users_name : SpecsFor<HomeController>
		{
			private ActionResult _result;

			protected override void When()
			{
				_result = SUT.SetName("Jane Doe");
			}

			[Test]
			public void then_it_sets_the_name_of_the_user()
			{
				GetMockFor<ICurrentUser>()
					.Verify(x => x.SetName("Jane Doe"));
			}

			[Test]
			public void then_it_redirects_back_home()
			{
				_result.ShouldRedirectTo<HomeController>(c => c.Index());
			}
		}

		public class when_saying_hello_to_a_user : SpecsFor<HomeController>
		{
			private ActionResult _result;

			protected override void When()
			{
				_result = SUT.SayHello("John Doe");
			}

			[Test]
			public void then_it_says_hello_to_the_user()
			{
				_result.ShouldRenderDefaultView()
					.WithModelLike(new SayHelloViewModel
					{
						Name = "John Doe"
					});
			}
		}

		public class when_saying_hello_with_a_form : SpecsFor<HomeController>
		{
			private ActionResult _result;

			protected override void When()
			{
				_result = SUT.SayHello(new SayHelloForm { Name = "Jane Doe" });
			}

			[Test]
			public void then_it_redirects_to_the_say_hello_action()
			{
				_result.ShouldRedirectTo<HomeController>(
					c => c.SayHello("Jane Doe"));
			}
		}
	}
}