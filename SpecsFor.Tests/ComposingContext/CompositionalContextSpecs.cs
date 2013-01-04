using System.Collections.Generic;
using NUnit.Framework;
using Should;
using SpecsFor.Tests.ComposingContext.TestDomain;

namespace SpecsFor.Tests.ComposingContext
{
	public class CompositionalContextSpecs
	{
		public class when_running_tests_decorated_with_a_behavior : SpecsFor<Widget>, ILikeMagic
		{
			public List<string> CalledByDuringGiven { get; set; }
			public List<string> CalledByAfterTest { get; set; }

			public when_running_tests_decorated_with_a_behavior()
			{
				CalledByDuringGiven = new List<string>();
				CalledByAfterTest = new List<string>();
			}

			[Test]
			public void then_handlers_for_the_interface_are_called()
			{
				CalledByDuringGiven.ShouldContain(typeof(ProvideMagicByInterface).Name);
			}

			[Test]
			public void then_handlers_for_the_type_are_called()
			{
				CalledByDuringGiven.ShouldContain(typeof (ProvideMagicByConcreteType).Name);
			}

			[Test]
			public void then_handlers_for_names_are_called()
			{
				CalledByDuringGiven.ShouldContain(typeof (ProvideMagicByTypeName).Name);
			}

			[Test]
			public void then_handlers_that_do_not_match_should_not_be_called()
			{
				CalledByDuringGiven.ShouldNotContain(typeof (DoNotProvideMagic).Name);
			}

			[Test]
			public void then_handlers_for_all_types_are_called()
			{
				CalledByDuringGiven.ShouldContain(typeof(ProvideMagicForEveryone).Name);
			}

			public override void TearDown()
			{
				base.TearDown();

				//At this point, all the AfterSpec contexts should be applied.
				CalledByAfterTest.ShouldContain(typeof(ProvideMagicByInterface).Name);
				CalledByAfterTest.ShouldContain(typeof(ProvideMagicByConcreteType).Name);
				CalledByAfterTest.ShouldContain(typeof(ProvideMagicByTypeName).Name);
				CalledByAfterTest.ShouldContain(typeof(ProvideMagicForEveryone).Name);
				CalledByAfterTest.ShouldNotContain(typeof(DoNotProvideMagic).Name);
			}
		}
	}
}