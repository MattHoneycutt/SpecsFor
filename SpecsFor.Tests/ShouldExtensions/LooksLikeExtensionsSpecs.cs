using System;
using NUnit.Framework;

namespace SpecsFor.Tests.ShouldExtensions
{
	public class LooksLikeExtensionsSpecs : SpecsFor<LooksLikeExtensionsSpecs.TestObject>
	{
		#region Test Class

		public class TestObject
		{
			public int ID { get; set; }
			public string Name { get; set; }
		}

		#endregion

		protected override void InitializeClassUnderTest()
		{
			SUT = new TestObject {ID = 1, Name = "Test"};
		}

		[Test]
		public void two_equivalent_objects_look_identical()
		{
			Assert.DoesNotThrow(() =>
			                    SUT.ShouldLookLike(new TestObject {ID = 1, Name = "Test"}));
		}

		[Test]
		public void two_different_objects_should_not_look_the_same()
		{
			Assert.Throws<Exception>(() => SUT.ShouldLookLike(new TestObject()));
		}
	}
}