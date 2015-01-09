using NUnit.Framework;
using Should;
using SpecsFor;
using SpecsFor.Helpers.Web.Mvc;
using SpecsForWebHelpers.Web.Helpers;

namespace SpecsForWebHelpers.Specs.Fakes
{
	public class ConfiguringBehaviorDemos
	{
		public class when_getting_the_app_version_and_the_app_is_in_debug_mode : SpecsFor<FakeHtmlHelper>
		{
			private string _version;

			protected override void Given()
			{
				GetMockFor<IHttpContextBehavior>()
					.Setup(x => x.IsDebuggingEnabled)
					.Returns(true);
			}

			protected override void When()
			{
				_version = SUT.GetVersionString();
			}

			[Test]
			public void then_the_version_contains_the_debug_suffix()
			{
				_version.EndsWith("DEBUG").ShouldBeTrue();
			}
		}
		
		public class when_getting_the_app_version_and_the_app_is_in_release_mode : SpecsFor<FakeHtmlHelper>
		{
			private string _version;

			protected override void Given()
			{
				GetMockFor<IHttpContextBehavior>()
					.Setup(x => x.IsDebuggingEnabled)
					.Returns(false);
			}

			protected override void When()
			{
				_version = SUT.GetVersionString();
			}

			[Test]
			public void then_the_version_contains_the_debug_suffix()
			{
				_version.EndsWith("DEBUG").ShouldBeFalse();
			}
		}
	}
}