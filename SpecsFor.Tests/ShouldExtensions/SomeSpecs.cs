using NUnit.Framework;
using Should.Core.Exceptions;
using SpecsFor;
using SpecsFor.ShouldExtensions;

namespace SpecsFor.Tests.ShouldExtensions
{
	public class SomeSpecs
	{
		public class TestObject
		{
			public int IntValue { get; set; }
		}

		public class when_checking_that_a_value_is_in_range : SpecsFor<TestObject>
		{
			[Test]
			public void then_it_does_not_throw_for_a_matching_inclusive_check()
			{
				var obj = new TestObject {IntValue = 5};
				Assert.DoesNotThrow(() =>
					obj.ShouldLookLike(() => new TestObject
					{
						IntValue = Some.ValueInRange(5, 5)
					})
					);
			}

			[Test]
			public void then_it_throws_for_a_non_matching_inclusive_check()
			{
				var obj = new TestObject {IntValue = 4};
				Assert.Throws<EqualException>(() =>
					obj.ShouldLookLike(() => new TestObject
					{
						IntValue = Some.ValueInRange(5, 10)
					})
					);
			}

			[Test]
			public void then_it_does_not_throw_for_a_matching_exclusive_check()
			{
				var obj = new TestObject {IntValue = 5};
				Assert.DoesNotThrow(() =>
					obj.ShouldLookLike(() => new TestObject
					{
						IntValue = Some.ValueInRange(4, 6, false)
					})
					);
			}

			[Test]
			public void then_it_throws_for_a_non_matching_exclusive_check()
			{
				var obj = new TestObject {IntValue = 4};
				Assert.Throws<EqualException>(() =>
					obj.ShouldLookLike(() => new TestObject
					{
						IntValue = Some.ValueInRange(4, 10, false)
					})
					);
			}
		}
	}
}