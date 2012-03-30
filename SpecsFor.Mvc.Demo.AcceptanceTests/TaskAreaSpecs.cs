using NUnit.Framework;
using SpecsFor.Mvc.Demo.Areas.Tasks.Controllers;
using Should;

namespace SpecsFor.Mvc.Demo.AcceptanceTests
{
	public class TaskAreaSpecs
	{
		public class when_navigating_to_an_area : SpecsFor<MvcWebApp>
		{
			protected override void When()
			{
				SUT.NavigateTo<ListController>(c => c.Index());				
			}

			[Test]
			public void then_it_correctly_locates_the_action()
			{
				SUT.Browser.Title.ShouldEqual("Tasks");
			}
		}
	}
}