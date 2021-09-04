using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;
using SpecsFor.StructureMap;

namespace SpecsFor.Shouldly.Tests.ShouldlyTests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "BDD naming style")]
    public class PartialMatchingWithListsSpecs
	{
		#region Test Classes
		public class TestClass
		{
			public string Name { get; set; }
			public List<NestedClass> Items { get; set; }
		}

		public class NestedClass
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}
		#endregion

		public class when_partial_matching_objects_with_list_members : SpecsFor<TestClass>
		{
			[Test]
			public void then_it_does_not_throw_on_a_matching_pair_of_objects()
			{
				var obj1 = new TestClass { Name = "Name", Items = new List<NestedClass> { new NestedClass { Id = 1, Name = "One" } } };

				Assert.DoesNotThrow(() => obj1.ShouldLookLike(() =>
					new TestClass { Name = "Name", Items = new List<NestedClass> { new NestedClass { Id = 1 } } })
					);
			}

			[Test]
			public void then_it_does_throw_on_a_non_matching_pair_of_objects()
			{
				var obj1 = new TestClass { Name = "Name", Items = new List<NestedClass> { new NestedClass { Id = 1, Name = "One" } } };

				Assert.Throws<ShouldAssertException>(() => obj1.ShouldLookLike(() =>
					new TestClass { Name = "Name", Items = new List<NestedClass> { new NestedClass { Id = 2 } } })
					);
			}
		}
	}
}
