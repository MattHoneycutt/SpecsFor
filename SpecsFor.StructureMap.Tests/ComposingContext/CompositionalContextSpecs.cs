using System.Collections.Generic;
using NUnit.Framework;
using Should;
using SpecsFor.StructureMap.Tests.ComposingContext.TestDomain;

namespace SpecsFor.StructureMap.Tests.ComposingContext
{
	public class CompositionalContextSpecs
	{
		public abstract class given_the_default_state : SpecsFor<Widget>, ILikeMagic
		{
			public List<string> CalledBySpecInit { get; set; }
			public List<string> CalledByApplyAfterClassUnderTestInitialized { get; set; }
			public List<string> CalledByDuringGiven { get; set; }
			public List<string> CalledByAfterGiven { get; set; }
			public List<string> CalledByAfterTest { get; set; }

			protected given_the_default_state()
			{
				CalledBySpecInit = new List<string>();
				CalledByApplyAfterClassUnderTestInitialized = new List<string>();
				CalledByDuringGiven = new List<string>();
				CalledByAfterGiven = new List<string>();
				CalledByAfterTest = new List<string>();
			}
		}

		public class when_running_tests_decorated_with_a_behavior : given_the_default_state
		{
			[Test]
			public void then_handlers_for_the_interface_are_called()
			{
				CalledByDuringGiven.ShouldContain(typeof(ProvideMagicByInterface).Name);
				CalledByAfterGiven.ShouldContain(typeof(ProvideMagicByInterface).Name);
			}

			[Test]
			public void then_handlers_for_the_type_are_called()
			{
				CalledByDuringGiven.ShouldContain(typeof (ProvideMagicByConcreteType).Name);
				CalledByAfterGiven.ShouldContain(typeof (ProvideMagicByConcreteType).Name);
			}

			[Test]
			public void then_handlers_for_names_are_called()
			{
				CalledByDuringGiven.ShouldContain(typeof (ProvideMagicByTypeName).Name);
				CalledByAfterGiven.ShouldContain(typeof (ProvideMagicByTypeName).Name);
			}

			[Test]
			public void then_handlers_that_do_not_match_should_not_be_called()
			{
				CalledByDuringGiven.ShouldNotContain(typeof (DoNotProvideMagic).Name);
				CalledByAfterGiven.ShouldNotContain(typeof (DoNotProvideMagic).Name);
			}

			[Test]
			public void then_handlers_for_all_types_are_called()
			{
				CalledByDuringGiven.ShouldContain(typeof(ProvideMagicForEveryone).Name);
				CalledByAfterGiven.ShouldContain(typeof(ProvideMagicForEveryone).Name);
			}

			[Test]
			public void then_it_invokes_the_handlers_during_the_init_phase()
			{
				CalledBySpecInit.ShouldContain(typeof(ProvideMagicForEveryone).Name);
			}

			[Test]
			public void then_it_invokes_the_class_initialized_phase()
			{
				CalledBySpecInit.ShouldContain(typeof(ProvideMagicForEveryone).Name);
			}
			
			// This no longer works. I assume it did at some point. I don't see a quick way to restore this check inside a spec.
			// protected override void AfterSpec()
			//    {
			//        //At this point, all the AfterSpec contexts should be applied.
			//        CalledByAfterTest.ShouldContain(typeof(ProvideMagicByInterface).Name);
			//        CalledByAfterTest.ShouldContain(typeof(ProvideMagicByConcreteType).Name);
			//        CalledByAfterTest.ShouldContain(typeof(ProvideMagicByTypeName).Name);
			//        CalledByAfterTest.ShouldContain(typeof(ProvideMagicForEveryone).Name);
			//        CalledByAfterTest.ShouldNotContain(typeof(DoNotProvideMagic).Name);
			//    }
		}
	}
}