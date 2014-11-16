using Beginners.Domain;
using SpecsFor;
using NUnit.Framework;
using Should;

namespace Beginners
{
	public class CalculatorSpecs
	{
		public class when_adding_two_numbers : SpecsFor<Calculator>
		{
			private int _result;

			protected override void When()
			{
				_result = SUT.Add(1, 2);
			}

			[Test]
			public void then_the_result_should_be_three()
			{
				_result.ShouldEqual(3);
			}
		}
	}
}