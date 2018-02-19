using NUnit.Framework;
using Should;

namespace SpecsFor.Mvc.Specs
{
	public class SpecsForMvcConfigSpecs
	{
		public class when_pointing_to_a_static_URL : SpecsFor<Mvc.SpecsForMvcConfig>
        {
			private const string AppUrl = "http://some.server.com/somePath";

            public override void SetupEachSpec()
            {
                base.SetupEachSpec();

                foreach (var action in SUT.TestRunnerActions)
                {
                    action.Startup();
                }
            }

            protected override void When()
			{
				SUT.UseApplicationAtUrl(AppUrl);
			}

			[Test]
			public void then_it_configures_MVC_web_app()
			{
				MvcWebApp.BaseUrl.ShouldEqual(AppUrl);
			}
		}
	}
}