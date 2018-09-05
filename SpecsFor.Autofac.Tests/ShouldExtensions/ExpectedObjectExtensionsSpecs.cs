using NUnit.Framework;
using SpecsFor.Core.ShouldExtensions;

namespace SpecsFor.Autofac.Tests.ShouldExtensions
{
	public class ExpectedObjectExtensionsSpecs : SpecsFor<ExpectedObjectExtensionsSpecs.TestObject>
	{
		#region Test Classes

		public class TestObject
		{
			public int ID { get; set; }
			public string Name { get; set; }
		}

		public interface ITestService
		{
			void DoStuff(TestObject obj);
		}

		#endregion

		protected override void InitializeClassUnderTest()
		{
			SUT = new TestObject { ID = 1, Name = "Test" };
		}

		[Test]
		public void two_equivalent_objects_look_identical()
		{
			Assert.DoesNotThrow(() => SUT.ShouldLookLike(new TestObject { ID = 1, Name = "Test" }));
		}

		[Test]
		public void two_different_objects_do_not_look_the_same()
		{
			Assert.Throws<ExpectedObjects.ComparisonException>(() => SUT.ShouldLookLike(new TestObject()));
		}

		[Test]
		public void then_partial_matching_with_an_equivalenet_object_works()
		{
			Assert.DoesNotThrow(() => SUT.ShouldLookLikePartial(new { ID = 1, Name = "Test" }));
		}

		[Test]
		public void then_partial_matching_with_an_unequivalenet_object_throws_exception()
		{
			Assert.Throws<ExpectedObjects.ComparisonException>(() => SUT.ShouldLookLikePartial(new { ID = 5, Name = "blah" }));
		}
	}
}