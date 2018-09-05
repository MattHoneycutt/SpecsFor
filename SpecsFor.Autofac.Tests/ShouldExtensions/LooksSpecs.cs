using System;
using Moq;
using NUnit.Framework;
using SpecsFor.Core.ShouldExtensions;

namespace SpecsFor.Autofac.Tests.ShouldExtensions
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

		[Test]
		public void moq_will_match_on_a_partial_object()
		{
			var mock = GetMockFor<ITestService>();

			mock.Object.DoStuff(new TestObject {ID = 1, Name = "Test"});

			Assert.DoesNotThrow(() => mock.Verify(s => s.DoStuff(Looks.LikePartialOf<TestObject>(new {ID = 1, Name = "Test"}))));
		}

		[Test]
		public void moq_will_not_match_on_a_nonequivalent_partial_object()
		{
			var mock = GetMockFor<ITestService>();

			mock.Object.DoStuff(new TestObject {ID = 3, Name = "Name"});

			Assert.Throws<MockException>(() => mock.Verify(s => s.DoStuff(Looks.LikePartialOf<TestObject>(new {ID = 1, Name = "Not Name"}))));
		}

		[Test]
		public void moq_will_match_on_a_strongly_typed_partial_object()
		{
			var mock = GetMockFor<ITestService>();

			mock.Object.DoStuff(new TestObject { ID = 1, Name = "Test" });

			Assert.DoesNotThrow(() => mock.Verify(s => s.DoStuff(Looks.Like(() => new TestObject { ID = 1, Name = "Test" }))));
		}

		[Test]
		public void moq_will_not_match_on_a_strongly_typed_partial_object_that_differs()
		{
			var mock = GetMockFor<ITestService>();

			mock.Object.DoStuff(new TestObject { ID = 1, Name = "Test" });

			Assert.Throws<MockException>(() => mock.Verify(s => s.DoStuff(Looks.Like(() => new TestObject { ID = 2 }))));
		}

		[Test]
		public void then_it_will_match_on_a_strongly_typed_partial_object_with_a_matching_expression()
		{
			var mock = GetMockFor<ITestService>();

			mock.Object.DoStuff(new TestObject { ID = 1, Name = "Test" });

			Assert.DoesNotThrow(() => mock.Verify(s => s.DoStuff(Looks.Like(() => new TestObject { ID = Some.ValueOf<int>(x => x == 1) }))));
		}

		[Test]
		public void then_it_will_not_match_on_a_strongly_typed_partial_object_with_a_partial_matcher_that_should_not_match()
		{
			var mock = GetMockFor<ITestService>();

			mock.Object.DoStuff(new TestObject { ID = 1, Name = "Test" });

            //TODO: The error message here sucks.  Something in Moq's internals is blowing up when the expression doesn't match.
			Assert.Throws<InvalidOperationException>(() => mock.Verify(s => s.DoStuff(Looks.Like(() => new TestObject { ID = Some.ValueOf<int>(x => x == 2) }))));
		}
	}
}