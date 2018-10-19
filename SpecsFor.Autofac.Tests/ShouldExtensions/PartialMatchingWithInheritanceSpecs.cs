using System;
using NUnit.Framework;
using Should;
using Should.Core.Exceptions;
using SpecsFor.Core.ShouldExtensions;

namespace SpecsFor.Autofac.Tests.ShouldExtensions
{
	public class PartialMatchingWithInheritanceSpecs : SpecsFor<PartialMatchingWithInheritanceSpecs.TestObject>
	{
		#region Test Classes
		public class BaseTestObject
		{
			public virtual int Value { get; set; }
			public int ParentOnly { get; set; }
		}

		public class TestObject : BaseTestObject
		{
			public override int Value { get; set; }
			public int SUTOnly { get; set; }
			public virtual int SUTValue { get; set; }
		}

		public class ChildTestObject : TestObject
		{
			public int ChildOnly { get; set; }
			public override int SUTValue { get;set; }
		}
		#endregion

		protected override void InitializeClassUnderTest()
		{
			SUT = new TestObject
			{
				Value = 10,
				SUTOnly = 11,
				ParentOnly = 12,
				SUTValue = 13
			};
		}

		[Test]
		public void then_it_should_like_parent_classes_with_matching_values()
		{
			Assert.DoesNotThrow(() => SUT.ShouldLookLike(() => new BaseTestObject
			{
				Value = 10,
				ParentOnly = 12
			}));
		}

		[Test]
		public void then_it_should_like_identical_class_with_matching_values()
		{
			Assert.DoesNotThrow(() => SUT.ShouldLookLike(() => new TestObject
			{
				Value = 10,
				SUTOnly = 11,
				ParentOnly = 12,
				SUTValue = 13
			}));
		}

		[Test]
		public void then_it_should_like_child_classes_with_matching_values()
		{
			Assert.DoesNotThrow(() => SUT.ShouldLookLike(() => new ChildTestObject
			{
				Value = 10,
				ParentOnly = 12,
				SUTValue = 13,
				SUTOnly = 11
			}));
		}

		[Test]
		public void then_it_should_fail_when_inherited_values_differ()
		{
			Assert.Throws<EqualException>(() => SUT.ShouldLookLike(() => new BaseTestObject
			{
				Value = 20
			}));
		}

		[Test]
		public void then_it_should_fail_when_parent_values_differ()
		{
			Assert.Throws<EqualException>(() => SUT.ShouldLookLike(() => new BaseTestObject
			{
				ParentOnly = 20
			}));
		}

		[Test]
		public void then_it_should_throw_when_property_to_check_does_not_exist_on_actual_object()
		{
			var exception = Assert.Throws<InvalidOperationException>(() => SUT.ShouldLookLike(() => new ChildTestObject
			{
				ChildOnly = 12
			}));
			exception.Message.ShouldContain("Unable to find property 'ChildOnly' on actual object");
		}
	}
}