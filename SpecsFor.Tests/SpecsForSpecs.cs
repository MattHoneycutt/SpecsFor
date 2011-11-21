using NUnit.Framework;
using Should;

namespace SpecsFor.Tests
{
	public class SpecsForSpecs
	{
		public class when_running_specs_with_no_given_and_no_when : SpecsFor<object>
		{
			[Test]
			public void then_the_SUT_is_still_initialized()
			{
				SUT.ShouldNotBeNull();
			}
		}

		public class when_running_specs_with_multiple_thens : SpecsFor<object>
		{
			private static int _whenCount;
			private static int _givenCount;

			protected override void Given()
			{
				_givenCount++;
			}

			protected override void When()
			{
				_whenCount++;
			}

			[Test]
			public void then_the_when_should_only_be_executed_once()
			{
				_whenCount.ShouldEqual(1);
			}

			[Test]
			public void then_the_when_should_still_only_be_executed_once()
			{
				_whenCount.ShouldEqual(1);
			}

			[Test]
			public void then_the_given_should_only_be_executed_once()
			{
				_givenCount.ShouldEqual(1);
			}

			[Test]
			public void then_the_given_should_still_only_be_executed_once()
			{
				_givenCount.ShouldEqual(1);
			}
		}
	}
}