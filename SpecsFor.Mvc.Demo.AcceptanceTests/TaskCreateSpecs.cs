using NUnit.Framework;
using Should;
using SpecsFor.Mvc.Demo.Areas.Tasks.Controllers;
using SpecsFor.Mvc.Demo.Areas.Tasks.Models;
using SpecsFor.Mvc.Demo.Controllers;
using SpecsFor.Mvc.Demo.Models;
using SpecsFor.Mvc.Helpers;

namespace SpecsFor.Mvc.Demo.AcceptanceTests
{
	public class TaskCreateSpecs
	{
		public class when_creating_a_complete_task : SpecsFor<MvcWebApp>
		{
			protected override void Given()
			{
                SUT.NavigateTo<ListController>(c => c.Index());
                SUT.FindLinkTo<ListController>(c => c.Create()).Click();
			}

			protected override void When()
			{
				SUT.FindFormFor<Task>()
					.Field(m => m.Title).SetValueTo("use radio buttons")
					.Field(m => m.Complete, true).Click()
					.Submit();
			}

			[Test]
			public void then_it_should_redirect_to_index()
			{
                SUT.Route.ShouldMapTo<ListController>(c => c.Index());
			}
		}

        public class when_creating_an_incomplete_task : SpecsFor<MvcWebApp>
        {
            protected override void Given()
            {
                SUT.NavigateTo<ListController>(c => c.Index());
                SUT.FindLinkTo<ListController>(c => c.Create()).Click();
            }

            protected override void When()
            {
                SUT.FindFormFor<Task>()
                    .Field(m => m.Title).SetValueTo("use radio buttons")
                    .Field(m => m.Complete, "false").Click()
                    .Submit();
            }

            [Test]
            public void then_it_should_redirect_to_index()
            {
                SUT.Route.ShouldMapTo<ListController>(c => c.Create());
            }
        }

        public class when_creating_a_high_priority_task : SpecsFor<MvcWebApp>
        {
            protected override void Given()
            {
                SUT.NavigateTo<ListController>(c => c.Index());
                SUT.FindLinkTo<ListController>(c => c.Create()).Click();
            }

            protected override void When()
            {
                SUT.FindFormFor<Task>()
                    .Field(m => m.Title).SetValueTo("use radio buttons")
					.Field(m => m.Complete, true).Click()
                    .Field(m => m.Priority, Priority.High).Click()
                    .Submit();
            }

            [Test]
            public void then_it_should_redirect_to_index()
            {
                SUT.Route.ShouldMapTo<ListController>(c => c.Index());
            }
        }
	}
}