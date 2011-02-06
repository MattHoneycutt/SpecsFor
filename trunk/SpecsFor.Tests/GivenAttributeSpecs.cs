using System;
using NUnit.Framework;
using Should;

namespace SpecsFor.Tests
{
	public class GivenAttributeSpecs
	{
		[Given()]
		public class when_constructing_the_attribute : SpecsFor<GivenAttribute>
		{
			private InvalidContextException _exception;

			public when_constructing_the_attribute(Type[] context) : base(context){}

			protected override void When()
			{
				_exception = Assert.Throws<InvalidContextException>(() => new GivenAttribute(typeof (int), typeof (string)));
			}

			[Test]
			public void then_the_exception_contains_the_names_of_all_bad_types()
			{
				Console.WriteLine(_exception.Message);
				_exception.Message.ShouldContain("Int32");
				_exception.Message.ShouldContain("String");
			}
		}
	}	
}