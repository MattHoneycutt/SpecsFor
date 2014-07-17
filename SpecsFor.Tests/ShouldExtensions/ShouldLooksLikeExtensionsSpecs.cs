using System;
using NUnit.Framework;
using Should.Core.Exceptions;
using SpecsFor.ShouldExtensions;

namespace SpecsFor.Tests.ShouldExtensions
{
	public class ShouldLooksLikeExtensionsSpecs : SpecsFor<ShouldLooksLikeExtensionsSpecs.TestObject>
	{
		public class TestObject
		{
			public Guid TestObjectId { get; set; }
			public int Awesomeness { get; set; }
			public string Name { get; set; }
			public TestObject Nested { get; set; }
			public TestObject[] NestedArray { get; set; }
		}

		protected override void InitializeClassUnderTest()
		{
			SUT = new TestObject { TestObjectId = Guid.NewGuid(), Name = "Test", Awesomeness = 11};
		}

		[Test]
		public void then_it_should_only_check_specified_properties()
		{
			Assert.DoesNotThrow(() => SUT.ShouldLookLike(() => new TestObject
			{
				Name = "Test",
				Awesomeness = 11
			}));
		}

		[Test]
		public void then_it_should_like_guids_too()
		{
			Assert.DoesNotThrow(() => SUT.ShouldLookLike(() => new TestObject
			{
				TestObjectId = SUT.TestObjectId
			}));
		}

		[Test]
		public void then_it_should_fail_if_specified_properties_dont_match()
		{
			Assert.Throws<EqualException>(() => SUT.ShouldLookLike(() => new TestObject
			{
				Name = "Tests"
			}));
		}

		[Test]
		public void then_it_should_work_with_nested_objects()
		{
			SUT.Nested = new TestObject
			{
				Name = "nested 1 test",
				Awesomeness = -10, //not going to specify in assertion
				Nested = new TestObject
				{
					Name = "ULTRA NEST COMBO KILL",
					Awesomeness = 69 //thanks, Bill & Ted, real mature.
				}
			};

			Assert.DoesNotThrow(() => SUT.ShouldLookLike(() => new TestObject
			{
				Name = "Test",
				Awesomeness = 11,
				Nested = new TestObject
				{
					Name = "nested 1 test",
					Nested = new TestObject
					{
						Name = "ULTRA NEST COMBO KILL",
						Awesomeness = 69
					}
				}
			}));
		}

		[Test]
		public void then_it_should_work_with_ienumerables()
		{
			Assert.Inconclusive("write this code");
		}
	}
}