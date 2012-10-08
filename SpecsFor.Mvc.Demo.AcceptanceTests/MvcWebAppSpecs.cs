using NUnit.Framework;
using SpecsFor.Mvc.Demo.Controllers;
using Should;

namespace SpecsFor.Mvc.Demo.AcceptanceTests
{
	public class MvcWebAppSpecs
	{
		public class when_getting_all_text_for_the_page : SpecsFor<MvcWebApp>
		{
			protected override void When()
			{
				SUT.NavigateTo<HomeController>(c => c.About());
			}

			[Test]
			public void then_you_can_get_the_full_page_text()
			{
				SUT.AllText().ShouldContain("Our business days are");
			}
		}
	}
}