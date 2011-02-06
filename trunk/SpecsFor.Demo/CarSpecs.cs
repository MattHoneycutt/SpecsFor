using System;
using NUnit.Framework;
using SpecsFor.Tests.TestObjects;

namespace SpecsFor.Tests
{
	public class CarSpecs
	{
		[Given(typeof(TheCarIsNotRunning), typeof(TheCarIsParked))]
		[Given(typeof(TheCarIsNotRunning))]
		public class when_the_key_is_turned : SpecsFor<Car>
		{
			public when_the_key_is_turned(Type[] context) : base(context){}

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

		public class when_the_key_is_turned_alternate_style : SpecsFor<Car>
		{
			protected override void Given()
			{
				Given<TheCarIsNotRunning>();
				Given<TheCarIsParked>();

				base.Given();
			}

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

		public class TheCarIsNotRunning : IContext<Car>
		{
			public void Initialize(ITestState<Car> state)
			{
				Console.WriteLine("In TheCarIsNotRunning!");
			}
		}

		public class TheCarIsParked : IContext<Car>
		{
			public void Initialize(ITestState<Car> state)
			{
				Console.WriteLine("In TheCarIsParked!");
			}
		}

		#endregion
	}
}