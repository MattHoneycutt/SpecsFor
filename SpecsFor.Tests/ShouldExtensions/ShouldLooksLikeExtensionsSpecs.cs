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
		}

		protected override void InitializeClassUnderTest()
		{
			SUT = new TestObject { TestObjectId = Guid.NewGuid(), Name = "Test", Awesomeness = 11};
		}

		[Test]
		public void then_it_should_only_check_specified_properties_good()
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
		public void then_it_should_only_check_specified_properties_bad()
		{
			Assert.Throws<EqualException>(() => SUT.ShouldLookLike(() => new TestObject
			{
				Name = "Tests"
			}));
		}
		
		//TODO: Handle nested properties, arrays, etc
	}
}