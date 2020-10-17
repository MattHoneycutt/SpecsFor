using NUnit.Framework;
using SpecsFor.StructureMap;

namespace SpecsFor.Should.Tests.ShouldTests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "BDD naming style")]
    public class StringExtensionsSpecs
	{
		public class when_checking_for_multiple_strings : SpecsFor<string>
		{
			protected override void InitializeClassUnderTest()
			{
				SUT = "Test1 Test2 Test3";
			}

			[Test]
			public void then_it_passes_when_the_string_contains_all_the_arguments()
			{
				SUT.ShouldContainAll("Test1", "Test2", "Test3");
			}

			[Test]
			public void then_it_throws_if_the_string_is_missing_any_argument()
			{
				Assert.Throws<AssertionException>(() => SUT.ShouldContainAll("Test1", "Test4"));
			}
		}
	}
}
