using Moq;
using NUnit.Framework;
using SpecsFor;
using SpecsFor.ShouldExtensions;

namespace Beginners.PartialMatching
{
	public class ShouldLookLikeSpecs
	{
		public class when_partially_comparing_a_simple_object : SpecsFor<object>
		{
			[Test]
			public void then_the_objects_are_equal_if_the_specified_properties_are_equal()
			{
				var person = new Person {FirstName = "John", LastName = "Smith"};
				person.ShouldLookLike(() => new Person
				{
					LastName = "Smith"
				});
			}

			[Test]
			public void then_the_objects_are_equal_if_the_matcher_is_satisfied()
			{
				var person = new Person { FirstName = "John", LastName = "Smith" };
				person.ShouldLookLike(() => new Person
				{
					LastName = It.Is<string>(s => s.StartsWith("S"))
				});				
			}
		}
	}
}