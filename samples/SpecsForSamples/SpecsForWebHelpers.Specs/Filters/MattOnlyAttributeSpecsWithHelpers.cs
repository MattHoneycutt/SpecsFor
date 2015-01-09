using NUnit.Framework;
using Should;
using SpecsFor;
using SpecsFor.Helpers.Web.Mvc;
using SpecsForWebHelpers.Web.Domain;
using SpecsForWebHelpers.Web.Filters;

namespace SpecsForWebHelpers.Specs.Filters
{
	public class MattOnlyAttributeSpecsWithHelpers
	{
		public class when_the_user_is_not_a_matt : SpecsFor<MattOnlyAttribute>
		{
			private FakeActionExecutingContext _filterContext;

			protected override void Given()
			{
				var userMock = GetMockFor<ICurrentUser>();
				userMock.Setup(x => x.UserName).Returns("John");
				SUT.CurrentUser = userMock.Object;
			}

			protected override void When()
			{
				_filterContext = new FakeActionExecutingContext();
				SUT.OnActionExecuting(_filterContext);
			}

			[Test]
			public void then_it_displays_an_unauthorized_view()
			{
				_filterContext.Result.ShouldRenderView()
					.ViewName.ShouldEqual("YouAreNotMatt");
			}
		}

		public class when_the_user_is_a_matt : SpecsFor<MattOnlyAttribute>
		{
			private FakeActionExecutingContext _filterContext;

			protected override void Given()
			{
				var userMock = GetMockFor<ICurrentUser>();
				userMock.Setup(x => x.UserName).Returns("Matt");
				SUT.CurrentUser = userMock.Object;
			}

			protected override void When()
			{
				_filterContext = new FakeActionExecutingContext();
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