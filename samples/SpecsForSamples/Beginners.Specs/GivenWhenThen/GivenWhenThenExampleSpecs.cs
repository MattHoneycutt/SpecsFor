using Beginners.Domain.MockingBasics;
using NUnit.Framework;
using Should;
using SpecsFor;

namespace Beginners.GivenWhenThen
{
	public class GivenWhenThenExampleSpecs
	{
		public class given_a_car_is_started_when_a_car_is_stopped : SpecsFor<Car>
		{
			protected override void Given()
			{
				//Given: establish state, in this case, that the car is started.
				SUT.Start();
			}

			protected override void When()
			{
				//When: perform an action, in this case, stopping the car.
				SUT.Stop();
			}

			//Note that there can be (and usually are) multiple 'Then' test cases.
			[Test]
			public void then_the_car_is_stopped()
			{
				//Then: the car is stopped.
				SUT.IsStopped.ShouldBeTrue();
			}

			[Test]
			public void then_the_engine_is_stopped()
			{
				//Then: the engine is stopped.
				SUT.Engine.IsStopped.ShouldBeTrue();
			}
		}
	}
}