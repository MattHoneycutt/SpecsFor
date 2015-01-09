using System.Runtime.InteropServices;
using System.Web.Mvc;
using NUnit.Framework;
using Should;
using SpecsFor;
using SpecsFor.ShouldExtensions;
using SpecsForWebHelpers.Web.Controllers;
using SpecsForWebHelpers.Web.Domain;
using SpecsForWebHelpers.Web.Models;

namespace SpecsForWebHelpers.Specs.Controllers
{
	public class HomeControllerSpecsWithoutHelpers
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
				var routeResult = (RedirectToRouteResult)_result;
				routeResult.RouteValues["action"].ShouldEqual("Index");
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
				var viewResult = _result.ShouldBeType<ViewResult>();
				var model = viewResult.Model.ShouldBeType<SayHelloViewModel>();
				model.ShouldLookLike(new SayHelloViewModel
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
				var redirectResult = _result.ShouldBeType<RedirectToRouteResult>();
				redirectResult.RouteValues["controller"].ShouldEqual("Home");
				redirectResult.RouteValues["action"].ShouldEqual("SayHello");
				redirectResult.RouteValues["name"].ShouldEqual("Jane Doe");
			}
		}
	}
}