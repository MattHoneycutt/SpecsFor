using System;
using NUnit.Framework;
using SpecsFor.Tests.TestObjects;
using Should;

namespace SpecsFor.Tests
{
	public class CarSpecs
	{
		public class when_the_key_is_turned1 : given.the_car_is_not_running
		{
			protected override void When()
			{
				SUT.TurnKey();
			}

			[Test]
			public void then_it_starts_the_engine()
			{
				GetMockFor<IEngine>()
					.Verify(e => e.Start());
			}
		}

		public static class given
		{
			public abstract class the_car_is_not_running : SpecsFor<Car>
			{
				protected override void Given()
				{
					base.Given();

					GetMockFor<IEngine>()
						.Setup(e => e.Start())
						.Callback(() => Console.WriteLine("VRRRROOOOOOOM"));
				}
			}

			public abstract class the_car_is_parked : SpecsFor<Car>
			{
				protected override void Given()
				{
					base.Given();

					GetMockFor<ITransmission>()
						.SetupProperty(t => t.Gear, "Park");
				}
			}

			public abstract class the_car_is_parked_and_not_running : the_car_is_not_running
			{
				//DUPLICATED CODE!  YUCK!
				protected override void Given()
				{
					base.Given();

					GetMockFor<ITransmission>()
						.SetupProperty(t => t.Gear, "Park");
				}
			}
		}


		#region Contexts

		public class TheCarIsRunning : IContext<Car>
		{
			public void Initialize(ITestState<Car> state)
			{
				state.GetMockFor<IEngine>()
					.Setup(e => e.Start())
					.Throws(new InvalidOperationException());
			}
		}

		#endregion
	}

	//Yuck, make it IContext, then verify that it implements the correct version of the generic interface?
	public interface Given<T1,T2> where T1 : IContext<T2>
	{
		
	}

	public class TheCarIsParked : IContext<Car>
	{
		public void Initialize(ITestState<Car> state)
		{
		}
	}

	public class GivenAttribute : TestFixtureAttribute
	{
		public GivenAttribute(params Type[] types) : base()
		{
			base.
			//TODO: Validate that they're all IContext types with default constructors? 
		}
	}
}