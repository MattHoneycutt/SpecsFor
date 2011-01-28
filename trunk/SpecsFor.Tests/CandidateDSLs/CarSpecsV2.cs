using System;
using NUnit.Framework;
using SpecsFor.Tests.TestObjects;
using Should;

namespace SpecsFor.Tests.CandidateDSLs
{
	public class CarSpecsV2
	{
		//This is one way to specify the context for a test: via code. 
		//Pros: No attributes, flexible
		//Cons: Doesn't read as well, more bloat in the spec class. 
		public class when_the_key_is_turned : SpecsFor<Car>
		{
			private InvalidOperationException _exception;

			protected override void Given()
			{
				Given<TheCarIsRunning>();
				//Context could be combined by making multiple calls to Given<TContext>(). 
			}

			protected override void When()
			{
				_exception = Assert.Throws<InvalidOperationException>(() => SUT.TurnKey());
			}

			[Test]
			public void then_it_should_throw_exception()
			{
				_exception.ShouldNotBeNull();
			}
		}

		//Pros: Reads better, less clutter to the code, makes it possible to run a fixture for many contexts.
		//Cons: Runtime checking on type, specifying order is a bit odd, will probably require NUnit add-in.
		[Given(typeof(TheCarIsRunning))]
		[Given(typeof(TheCarIsParked), typeof(TheCarIsRunning))]
		public class when_the_key_is_turned2 : SpecsFor<Car>
		{
			private InvalidOperationException _exception;

			protected override void When()
			{
				_exception = Assert.Throws<InvalidOperationException>(() => SUT.TurnKey());
			}

			[Test]
			public void then_it_should_throw_exception()
			{
				_exception.ShouldNotBeNull();
			}
		}

		//Pros: Reads better, less clutter.
		//Cons: Runtime checking on types, can't run the same fixture for multiple contexts...
		public class when_the_key_is_turned3 
			: SpecsFor<Car>, Given<TheCarIsRunning>, Given<TheCarIsParked>
		{
			private InvalidOperationException _exception;

			protected override void When()
			{
				_exception = Assert.Throws<InvalidOperationException>(() => SUT.TurnKey());
			}

			[Test]
			public void then_it_should_throw_exception()
			{
				_exception.ShouldNotBeNull();
			}
		}

		//Pros: Reads pretty well, not much clutter, enables running same fixture for multiple contexts. 
		//Cons: A little more clutter in the class.  Could be encapsulated further. 
		[Given(typeof(TheCarIsRunning))]
		[Given(typeof(TheCarIsParked), typeof(TheCarIsRunning))]
		public class when_the_key_is_turned4 : SpecsFor<Car>
		{
			private InvalidOperationException _exception;

			public when_the_key_is_turned4(params Type[] context)
				: base (context)
			{
			}

			protected override void When()
			{
				_exception = Assert.Throws<InvalidOperationException>(() => SUT.TurnKey());
			}

			[Test]
			public void then_it_should_throw_exception()
			{
				_exception.ShouldNotBeNull();
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

		public class TheCarIsParked : IContext<Car>
		{
			public void Initialize(ITestState<Car> state)
			{
			}
		}

		#endregion
	}

	public interface Given<TContext> 
	{
		
	}


	public class GivenAttribute : TestFixtureAttribute
	{
		public GivenAttribute(params Type[] types) : base()
		{
			//TODO: Validate that they're all IContext types with default constructors? 
		}
	}
}