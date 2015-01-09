using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using Should;
using SpecsFor;
using SpecsForWebHelpers.Web.Domain;
using SpecsForWebHelpers.Web.Filters;

namespace SpecsForWebHelpers.Specs.Filters
{
	public class MattOnlyAttributeSpecsWithoutHelpers
	{
		public class when_the_user_is_not_a_matt : SpecsFor<MattOnlyAttribute>
		{
			private ActionExecutingContext _filterContext;

			protected override void Given()
			{
				var userMock = GetMockFor<ICurrentUser>();
				userMock.Setup(x => x.UserName).Returns("John");
				SUT.CurrentUser = userMock.Object;
			}

			protected override void When()
			{
				var httpContext = new Mock<HttpContextBase>().Object;
				var controllerContext = new ControllerContext(httpContext, new RouteData(), new Mock<ControllerBase>().Object);
				var reflectedActionDescriptor = new ReflectedActionDescriptor(typeof(ControllerBase).GetMethods()[0], "Test", new ReflectedControllerDescriptor(typeof(ControllerBase)));
				_filterContext = new ActionExecutingContext(controllerContext, reflectedActionDescriptor, new Dictionary<string, object>());
				SUT.OnActionExecuting(_filterContext);
			}

			[Test]
			public void then_it_displays_an_unauthorized_view()
			{
				_filterContext.Result.ShouldBeType<ViewResult>()
					.ViewName.ShouldEqual("YouAreNotMatt");
			}
		}

		public class when_the_user_is_a_matt : SpecsFor<MattOnlyAttribute>
		{
			private ActionExecutingContext _filterContext;

			protected override void Given()
			{
				var userMock = GetMockFor<ICurrentUser>();
				userMock.Setup(x => x.UserName).Returns("Matt");
				SUT.CurrentUser = userMock.Object;
			}

			protected override void When()
			{
				var httpContext = new Mock<HttpContextBase>().Object;
				var controllerContext = new ControllerContext(httpContext, new RouteData(), new Mock<ControllerBase>().Object);
				var reflectedActionDescriptor = new ReflectedActionDescriptor(typeof(ControllerBase).GetMethods()[0], "Test", new ReflectedControllerDescriptor(typeof(ControllerBase)));
				_filterContext = new ActionExecutingContext(controllerContext, reflectedActionDescriptor, new Dictionary<string, object>());
				SUT.OnActionExecuting(_filterContext);
			}

			[Test]
			public void then_the_filter_does_not_alter_the_result()
			{
				_filterContext.Result.ShouldBeNull();
			}
		}

	}
}