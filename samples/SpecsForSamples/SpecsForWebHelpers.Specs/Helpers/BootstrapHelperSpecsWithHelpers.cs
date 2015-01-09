using System.Web.Routing;
using HtmlTags;
using NUnit.Framework;
using Should;
using SpecsFor;
using SpecsFor.Helpers.Web.Mvc;
using SpecsForWebHelpers.Web;
using SpecsForWebHelpers.Web.Controllers;
using SpecsForWebHelpers.Web.Helpers;

namespace SpecsForWebHelpers.Specs.Helpers
{
	public class BootstrapHelperSpecsWithHelpers
	{
		public class when_creating_a_bootstrap_button : SpecsFor<FakeHtmlHelper>
		{
			private HtmlTag _button;

			protected override void When()
			{
				_button = SUT.BootstrapButton("Submit!");
			}

			[Test]
			public void then_it_creates_submit_button()
			{
				_button.Attr("type").ShouldEqual("submit");
			}

			[Test]
			public void then_it_sets_the_correct_button_classes()
			{
				_button.HasClass("btn").ShouldBeTrue();
				_button.HasClass("btn-primary").ShouldBeTrue();
			}
		}

		public class when_creating_a_bootstrap_link_button : SpecsFor<FakeHtmlHelper>
		{
			private HtmlTag _link;

			protected override void Given()
			{
				RouteConfig.RegisterRoutes(RouteTable.Routes);
			}

			protected override void When()
			{
				_link = SUT.BootstrapActionLinkButton<HomeController>(
					c => c.SetName(), "Set Name!");
			}

			[Test]
			public void then_it_builds_a_link_tag()
			{
				_link.TagName().ShouldEqual("a");
			}

			[Test]
			public void then_it_sets_the_link_correctly()
			{
				_link.Attr("href").ShouldEqual("/Home/SetName");
			}

			[Test]
			public void then_it_looks_like_a_bootstrap_button()
			{
				_link.HasClass("btn").ShouldBeTrue();
				_link.HasClass("btn-primary").ShouldBeTrue();
			}
		}
	}
}