using Should;
using System.Security.Principal;
using System.Web.Mvc;
using NUnit.Framework;
using SpecsFor;
using SpecsFor.Helpers.Web.Mvc;

namespace SpecsFor.Helpers.Web.Specs
{
	public class FakeControllerContextSpecs
	{
		public class DummyController : Controller
		{
			
		}

		public class when_initializing_the_SUT_with_a_fake_controller : SpecsFor<DummyController>
		{
			protected override void When()
			{
				this.UseFakeContextForController();

				GetMockFor<IPrincipal>()
					.Setup(x => x.Identity.Name)
					.Returns("My test!");
			}

			[Test]
			public void then_it_configures_the_fake_user_correctly()
			{
				SUT.User.ShouldNotBeNull();
				SUT.User.Identity.Name.ShouldEqual("My test!");
			}
		}
	}
}