using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Should;
using SpecsFor.StructureMap.Tests.ComposingContext.TestDomain;

namespace SpecsFor.StructureMap.Tests.ComposingContext.StackingContext
{
	public class StackingContextSpecs
	{
		public class when_running_tests_decorated_with_a_behavior : SpecsFor<Widget>, ILikeMagic
		{
			public List<string> CalledByDuringGiven { get; set; }
			public List<string> CalledByAfterGiven { get; set; }
			public List<string> CalledByAfterSpec { get; set; }
			public List<string> CalledByAfterTest { get; set; }
			public List<string> CalledByBeforeTest { get; set; }
			public List<string> CalledByApplyAfterClassUnderTestInitialized { get; set; }
			public List<string> CalledBySpecInit { get; set; }

			public when_running_tests_decorated_with_a_behavior()
			{
				CalledBySpecInit = new List<string>();
				CalledByApplyAfterClassUnderTestInitialized = new List<string>();
				CalledByDuringGiven = new List<string>();
				CalledByAfterGiven = new List<string>();
				CalledByAfterSpec = new List<string>();
				CalledByAfterTest = new List<string>();
				CalledByBeforeTest = new List<string>();
			}

			[Test]
			public void then_handlers_defined_in_the_higher_level_config_should_be_called()
			{
				CalledByDuringGiven.ShouldContain(typeof(ProvideMagicByInterface).Name);
				CalledByDuringGiven.ShouldContain(typeof(ProvideMagicByConcreteType).Name);
				CalledByDuringGiven.ShouldContain(typeof(ProvideMagicByTypeName).Name);
				CalledByDuringGiven.ShouldContain(typeof(ProvideMagicForEveryone).Name);
				
				CalledByAfterGiven.ShouldContain(typeof(ProvideMagicByInterface).Name);
				CalledByAfterGiven.ShouldContain(typeof(ProvideMagicByConcreteType).Name);
				CalledByAfterGiven.ShouldContain(typeof(ProvideMagicByTypeName).Name);
				CalledByAfterGiven.ShouldContain(typeof(ProvideMagicForEveryone).Name);
			}

			[Test]
			public void then_doesnt_call_handlers_it_shouldnt()
			{
				CalledByDuringGiven.ShouldNotContain(typeof(DoNotProvideMagic).Name);
				CalledByAfterGiven.ShouldNotContain(typeof(DoNotProvideMagic).Name);
			}

			[Test]
			public void then_handlers_for_this_levels_config_should_be_called()
			{
				CalledByDuringGiven.ShouldContain(typeof(NestedMagicProvider).Name);
				CalledByAfterGiven.ShouldContain(typeof(NestedMagicProvider).Name);
			}

			[Test]
			public void then_the_parent_contexts_are_applied_before_the_child_context()
			{
				CalledByDuringGiven.AsEnumerable().Reverse().First().ShouldEqual(typeof(NestedMagicProvider).Name);
				CalledByAfterGiven.AsEnumerable().Reverse().First().ShouldEqual(typeof(NestedMagicProvider).Name);
			}
		}
	}
}