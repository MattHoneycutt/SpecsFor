using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using HtmlTags;
using Moq;
using NUnit.Framework;
using Should;
using SpecsFor;
using SpecsForWebHelpers.Web;
using SpecsForWebHelpers.Web.Controllers;
using SpecsForWebHelpers.Web.Helpers;

namespace SpecsForWebHelpers.Specs.Helpers
{
	public class BootstrapHelperSpecsWithoutHelpers
	{
		public class when_creating_a_bootstrap_button : SpecsFor<HtmlHelper>
		{
			private HtmlTag _button;

			protected override void InitializeClassUnderTest()
			{
				var contextMock = new Mock<HttpContextBase>();
				contextMock.Setup(x => x.Request).Returns(new Mock<HttpRequestBase>().Object);
				var response = new Mock<HttpResponseBase>();
				response.Setup(x => x.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);
				contextMock.Setup(x => x.Response).Returns(response.Object);
				var viewContext = new ViewContext()
				{
					HttpContext = contextMock.Object
				};
				var viewDataContainer = new Mock<IViewDataContainer>().Object;
				SUT = new HtmlHelper(viewContext, viewDataContainer);
			}

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

		public class when_creating_a_bootstrap_link_button : SpecsFor<HtmlHelper>
		{
			private HtmlTag _link;

			protected override void InitializeClassUnderTest()
			{
				var contextMock = new Mock<HttpContextBase>();
				contextMock.Setup(x => x.Request).Returns(new Mock<HttpRequestBase>().Object);
				var response = new Mock<HttpResponseBase>();
				response.Setup(x => x.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);
				contextMock.Setup(x => x.Response).Returns(response.Object);
				var viewContext = new ViewContext()
				{
					HttpContext = contextMock.Object
				};
				var viewDataContainer = new Mock<IViewDataContainer>().Object;
				SUT = new HtmlHelper(viewContext, viewDataContainer);
			}
			
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