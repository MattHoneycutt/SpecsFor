using NUnit.Framework;
using SpecsFor.Core.ShouldExtensions;

namespace SpecsFor.Autofac.Tests.ShouldExtensions
{
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