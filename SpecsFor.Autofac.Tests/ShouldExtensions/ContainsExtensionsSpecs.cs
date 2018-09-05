using NUnit.Framework;

namespace SpecsFor.Autofac.Tests.ShouldExtensions
{
	public class ContainsExtensionsSpecs : SpecsFor<string[]>
	{
		protected override void InitializeClassUnderTest()
		{
			SUT = new[] { "Test 1", "Test 2", "Test 3" };
		}

		[Test]
		public void ShouldContains_does_not_throw_if_list_contains_matching_item()
		{
			Assert.DoesNotThrow(() => SUT.ShouldContain(s => s.EndsWith("2")));
		}

		[Test]
		public void ShouldNotContains_throws_if_list_contains_matching_item()
		{
			Assert.Throws<AssertionException>(() => SUT.ShouldNotContain(s => s.EndsWith("2")));
		}

		[Test]
		public void ShouldContains_throws_if_list_does_not_contain_matching_item()
		{
			Assert.Throws<AssertionException>(() => SUT.ShouldContain(s => s.EndsWith("98")));
		}

		[Test]
		public void ShouldNotContains_does_not_throw_if_list_does_not_contain_matching_item()
		{
			Assert.DoesNotThrow(() => SUT.ShouldNotContain(s => s.EndsWith("98")));
		}
	}
}