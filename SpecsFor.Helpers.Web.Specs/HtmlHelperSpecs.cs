using System.Web.Mvc;
using System.Web.Mvc.Html;
using NUnit.Framework;
using SpecsFor.Helpers.Web.Mvc;
using SpecsFor.ShouldExtensions;

namespace SpecsFor.Helpers.Web.Specs
{
	public class HtmlHelperSpecs
	{
		public class when_testing_a_standard_html_helper : SpecsFor<FakeHtmlHelper>
		{
			private MvcHtmlString _result;

			protected override void When()
			{
				_result = SUT.Label("Name", new
				{
					@class = "col-md-2 control-label"
				});
			}

			[Test]
			public void then_it_returns_the_expected_markup()
			{
				_result.ToString().ShouldContainAll("col-md-2", "control-label");
			}
		}
	}
}