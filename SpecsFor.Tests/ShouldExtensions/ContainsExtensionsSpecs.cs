using System.Collections.Generic;
using NUnit.Framework;
using SpecsFor.ShouldExtensions;
using Assert = Should.Core.Assertions.Assert;
using Should;

namespace SpecsFor.Tests.ShouldExtensions
{
	public class ContainsExtensionsSpecs
	{
		public class when_checking_for_an_object_that_exists : given.an_enumerable_of_items
		{
			protected override void When()
			{
				Assert.DoesNotThrow(() => SUT.ShouldContain(s => s.EndsWith("2")));
			}

			[Test]
			public void then_it_does_not_throw_an_exception()
			{
				//Nothing to check.
			}
		}

		public class when_checking_for_an_object_that_does_not_exist : given.an_enumerable_of_items
		{
			private AssertionException _exception;

			protected override void When()
			{
				_exception = Assert.Throws<AssertionException>(() => SUT.ShouldContain(s => s.EndsWith("98")));
			}

			[Test]
			public void then_it_throws_an_exception()
			{
				_exception.ShouldNotBeNull();
			}
		}

		public static class given
		{
			public abstract class an_enumerable_of_items : SpecsFor<IEnumerable<string>>
			{
				protected override void InitializeClassUnderTest()
				{
					SUT = new[] {"Test 1", "Test 2", "Test 3"};
				}
			}
		}
	}
}