using System;
using Moq;
using NUnit.Framework;
using SpecsFor.ShouldExtensions;

namespace SpecsFor.Tests.ShouldExtensions
{
	public class LooksSpecs : SpecsFor<LooksSpecs.TestObject>
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
			SUT = new TestObject {ID = 1, Name = "Test"};
		}

		[Test]
		public void two_equivalent_objects_look_identical()
		{
			Assert.DoesNotThrow(() =>
			                    SUT.ShouldLookLike(new TestObject {ID = 1, Name = "Test"}));
		}

		[Test]
		public void two_different_objects_do_not_look_the_same()
		{
			Assert.Throws<Exception>(() => SUT.ShouldLookLike(new TestObject()));
		}

		[Test]
		public void moq_will_match_on_an_equivalent_object()
		{
			var mock = GetMockFor<ITestService>();

			mock.Object.DoStuff(new TestObject {ID = 1, Name = "Test"});

			Assert.DoesNotThrow(() => mock.Verify(s => s.DoStuff(Looks.Like(new TestObject {ID = 1, Name = "Test"}))));
		}

		[Test]
		public void moq_will_not_match_on_a_nonequivalent_object()
		{
			var mock = GetMockFor<ITestService>();

			mock.Object.DoStuff(new TestObject {ID = 3, Name = "Name"});

			Assert.Throws<MockException>(() => mock.Verify(s => s.DoStuff(Looks.Like(new TestObject {ID = 1, Name = "Not Name"}))));
		}
	}
}