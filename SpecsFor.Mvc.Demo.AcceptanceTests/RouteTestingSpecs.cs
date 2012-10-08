using NUnit.Framework;
using SpecsFor.Mvc.Demo.Areas.Tasks.Controllers;
using MvcContrib.TestHelper;

namespace SpecsFor.Mvc.Demo.AcceptanceTests
{
	public class RouteTestingSpecs
	{
		public class when_verifying_a_route_that_contains_query_string_parameters : SpecsFor<MvcWebApp>
		{
			protected override void When()
			{
				SUT.NavigateTo<ListController>(c => c.Create(5, "test"));
			}

			[Test]
			public void then_it_correctly_matches_the_same_route()
			{
				SUT.Route.ShouldMapTo<ListController>(c => c.Create(5, "test"));
			}

			[Test]
			public void then_it_throws_an_exception_on_a_different_route()
			{
				Assert.Throws<MvcContrib.TestHelper.AssertionException>(() => SUT.Route.ShouldMapTo<ListController>(c => c.Create(4, "other")));
			}
		}
	}
}