using Beginners.Domain.MockingBasics;
using NUnit.Framework;
using Should;
using SpecsFor;

namespace Beginners.MockingBasics
{
	public class CarFactorySpecs
	{
		public class when_creating_a_muscle_car : SpecsFor<CarFactory>
		{
			private Car _car;

			protected override void Given()
			{
				GetMockFor<IEngineFactory>()
					.Setup(x => x.GetEngine("V8"))
					.Returns(new Engine());
			}

			protected override void When()
			{
				_car = SUT.BuildMuscleCar();
			}

			[Test]
			public void then_it_creates_a_car_with_an_engine()
			{
				_car.Engine.ShouldNotBeNull();
			}

			[Test]
			public void then_it_calls_the_engine_factory()
			{
				GetMockFor<IEngineFactory>()
					.Verify(x => x.GetEngine("V8"));
			}
		}
	}
}