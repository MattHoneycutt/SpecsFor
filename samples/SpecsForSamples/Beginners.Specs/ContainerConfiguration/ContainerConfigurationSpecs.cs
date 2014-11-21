using Beginners.Domain.MockingBasics;
using NUnit.Framework;
using Should;
using SpecsFor;
using StructureMap;

namespace Beginners.ContainerConfiguration
{
	public class ContainerConfigurationSpecs
	{
		public class when_a_concrete_type_is_registered_in_the_container : SpecsFor<CarFactory>
		{
			private Car _car;

			protected override void ConfigureContainer(IContainer container)
			{
				container.Configure(cfg =>
				{
					cfg.For<IEngineFactory>().Use<RealEngineFactory>();
				});
			}

			protected override void When()
			{
				_car = SUT.BuildMuscleCar();
			}

			[Test]
			public void then_the_cars_engine_was_made_by_the_real_engine_factory()
			{
				_car.Engine.Maker.ShouldEqual("Real Engines, Inc");
			}
		}
	}
}