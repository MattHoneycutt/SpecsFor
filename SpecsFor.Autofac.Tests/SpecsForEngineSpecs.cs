using System;
using Moq;
using NUnit.Framework;
using Should;
using SpecsFor.Core;
using SpecsFor.Core.Configuration.Model;
using SpecsFor.Core.Validation;

namespace SpecsFor.Autofac.Tests
{
	internal class SpecsForEngineSpecs
	{
		internal abstract class given_there_are_specs_for_config_classes_that_do_not_have_the_required_attributes : SpecsFor<SpecsForEngine<object>>
		{
			protected override void Given()
			{
				GetMockFor<ISpecValidator>()
					.Setup(v => v.ValidateSpec(It.IsAny<ISpecs>()))
					.Throws(new InvalidOperationException());
			}

			public class when_initializing_the_engine : given_there_are_specs_for_config_classes_that_do_not_have_the_required_attributes
			{
				private InvalidOperationException _exception;

				protected override void When()
				{
					_exception = Assert.Throws<InvalidOperationException>(() => SUT.Init());
				}

				[Test]
				public void then_it_throws_an_exception()
				{
					_exception.ShouldNotBeNull();
				}
			}
		}

		internal class when_initializing_the_engine : SpecsFor<SpecsForEngine<object>>
		{
		    protected override void Given()
		    {
		        var mocker = GetMockFor<IAutoMocker>();

		        mocker.Setup(x => x.CreateSUT<object>()).Returns(new object());

		        GetMockFor<ISpecs<object>>().Setup(x => x.CreateAutoMocker()).Returns(mocker.Object);
		    }

            protected override void When()
			{
				SUT.Init();
			}

			[Test]
			public void then_it_applies_spec_init_behaviors()
			{
				GetMockFor<IBehaviorStack>()
					.Verify(s => s.ApplySpecInitTo(It.IsAny<ISpecs>()));
			}

			[Test]
			public void then_it_invokes_the_createautomocker_callback()
			{
				GetMockFor<ISpecs<object>>()
					.Verify(s => s.CreateAutoMocker());
			}

			[Test]
			public void then_it_invokes_the_initialize_class_under_test_callback()
			{
				GetMockFor<ISpecs<object>>()
					.Verify(s => s.InitializeClassUnderTest());
			}

			[Test]
			public void then_it_applies_after_class_under_test_initialized_behaviors()
			{
				GetMockFor<IBehaviorStack>()
					.Verify(s => s.ApplyAfterClassUnderTestInitializedTo(It.IsAny<ISpecs>()));
			}
		}

		public class when_initializing_the_engine_and_an_error_is_throwing_during_initialize_class_under_test : SpecsFor<SpecsForEngine<object>>
		{
			private SpecInitException _exception;

			protected override void Given()
			{
				GetMockFor<ISpecs<object>>()
					.Setup(s => s.InitializeClassUnderTest())
					.Throws(new Exception());
			}

			protected override void When()
			{
				_exception = Assert.Throws<SpecInitException>(() => SUT.Init());
			}

			[Test]
			public void then_it_throws_an_exception()
			{
				_exception.ShouldNotBeNull();
			}

			[Test]
			public void then_it_calls_the_after_spec_step_on_any_behaviors()
			{
				GetMockFor<IBehaviorStack>()
					.Verify(s => s.ApplyAfterSpecTo(It.IsAny<ISpecs>()));
			}
		}

		internal class when_initializing_the_class_under_test : given_an_initialization_behavior_is_defined
		{
			protected override void When()
			{
				SUT.Init();
				SUT.InitializeClassUnderTest();
			}

			[Test]
			public void then_it_creates_the_class_under_test()
			{
				SUT.SUT.ShouldNotBeNull();
			}
		}

		internal abstract class given_an_initialization_behavior_is_defined : SpecsFor<SpecsForEngine<object>>
		{
			private readonly object _expected = new object();

		    protected override void Given()
		    {
		        var mocker = GetMockFor<IAutoMocker>();

		        mocker.Setup(x => x.CreateSUT<object>()).Returns(new object());

		        GetMockFor<ISpecs<object>>().Setup(x => x.CreateAutoMocker()).Returns(mocker.Object);

                GetMockFor<IBehaviorStack>()
					.Setup(s => s.GetInitializationMethodFor(It.IsAny<ISpecs<object>>()))
					.Returns(() => _expected);
			}

			internal class when_initializing_the_class_under_test : given_an_initialization_behavior_is_defined
			{
				protected override void When()
				{
					SUT.Init();
					SUT.InitializeClassUnderTest();
				}

				[Test]
				public void then_it_uses_the_object_from_the_init_behavior()
				{
					SUT.SUT.ShouldBeSameAs(_expected);
				}
			}
		}

		internal class when_running_the_given_step : SpecsFor<SpecsForEngine<object>>
		{
			protected override void When()
			{
				SUT.Given();
			}

			[Test]
			public void then_it_invokes_the_given_of_the_spec()
			{
				GetMockFor<ISpecs<object>>()
					.Verify(s => s.Given());
			}

			[Test]
			public void then_it_invokes_the_given_behaviors()
			{
				GetMockFor<IBehaviorStack>()
					.Verify(s => s.ApplyGivenTo(It.IsAny<ISpecs<object>>()));
			}
		}

		internal abstract class given_an_error_is_thrown_by_the_spec_during_the_given_callback : SpecsFor<SpecsForEngine<IDisposable>>
		{
			protected override void Given()
			{
				GetMockFor<ISpecs<IDisposable>>()
					.Setup(s => s.Given())
					.Throws(new Exception());
			}

			internal class when_running_the_given_step : given_an_error_is_thrown_by_the_spec_during_the_given_callback
			{
				private GivenSpecificationException _exception;

				protected override void When()
				{
					SUT.SUT = GetMockFor<IDisposable>().Object;
					_exception = Assert.Throws<GivenSpecificationException>(() => SUT.Given());
				}

				[Test]
				public void then_it_rethrows_the_exception()
				{
					_exception.ShouldNotBeNull();
				}

				[Test]
				public void then_it_invokes_the_after_spec_callback()
				{
					GetMockFor<IBehaviorStack>()
						.Verify(s => s.ApplyAfterSpecTo(It.IsAny<ISpecs<IDisposable>>()));
				}

				[Test]
				public void then_it_disposes_of_the_system_under_test()
				{
					GetMockFor<IDisposable>()
						.Verify(d => d.Dispose());
				}
			}

			internal class when_running_the_given_step_and_multiple_exceptions_are_thrown : given_an_error_is_thrown_by_the_spec_during_the_given_callback
			{
				private GivenSpecificationException _exception;

				protected override void Given()
				{
					GetMockFor<IBehaviorStack>()
						.Setup(s => s.ApplyAfterSpecTo(It.IsAny<ISpecs<IDisposable>>()))
						.Throws(new InvalidOperationException("Other exception."));

					base.Given();
				}

				protected override void When()
				{
					SUT.SUT = GetMockFor<IDisposable>().Object;
					_exception = Assert.Throws<GivenSpecificationException>(() => SUT.Given());
				}

				[Test]
				public void then_it_throws_an_aggregate_exception_containing_both_exceptions()
				{
					_exception.Message.ShouldStartWith("An error occurred during the spec 'Given' phase.");
					_exception.Exceptions.Length.ShouldEqual(2);
				}
			}
		}

		internal class when_running_the_when_stage : SpecsFor<SpecsForEngine<object>>
		{
			protected override void When()
			{
				SUT.When();
			}

			[Test]
			public void then_it_calls_the_when_callback_on_the_spec()
			{
				GetMockFor<ISpecs<object>>()
					.Verify(s => s.When());
			}
		}

		internal abstract class given_an_error_is_thrown_by_the_spec_during_the_when_callback : SpecsFor<SpecsForEngine<IDisposable>>
		{
			protected override void Given()
			{
				GetMockFor<ISpecs<IDisposable>>()
					.Setup(s => s.When())
					.Throws(new Exception());
			}

			internal class when_running_the_when_stage : given_an_error_is_thrown_by_the_spec_during_the_when_callback
			{
				private WhenSpecificationException _exception;

				protected override void When()
				{
					SUT.SUT = GetMockFor<IDisposable>().Object;
					_exception = Assert.Throws<WhenSpecificationException>(() => SUT.When());
				}

				[Test]
				public void then_it_rethrows_the_exception()
				{
					_exception.ShouldNotBeNull();
				}

				[Test]
				public void then_it_disposes_the_system_under_test()
				{
					GetMockFor<IDisposable>()
						.Verify(d => d.Dispose());
				}

				[Test]
				public void then_it_invokes_the_after_spec_callback_on_the_behaviors()
				{
					GetMockFor<IBehaviorStack>()
						.Verify(s => s.ApplyAfterSpecTo(It.IsAny<ISpecs<IDisposable>>()));
				}
			}

			internal class when_running_the_when_stage_and_an_error_is_thrown_by_a_behaviors_after_spec : given_an_error_is_thrown_by_the_spec_during_the_when_callback
			{
				private WhenSpecificationException _exception;

				protected override void Given()
				{
					GetMockFor<IBehaviorStack>()
						.Setup(s => s.ApplyAfterSpecTo(It.IsAny<ISpecs<IDisposable>>()))
						.Throws(new InvalidOperationException());

					base.Given();
				}

				protected override void When()
				{
					SUT.SUT = GetMockFor<IDisposable>().Object;
					_exception = Assert.Throws<WhenSpecificationException>(() => SUT.When());
				}

				[Test]
				public void then_it_throws_an_aggregate_exception_containing_both_exceptions()
				{
					_exception.Message.StartsWith("An error occurred during the spec 'When' phase.");
					_exception.Exceptions.Length.ShouldEqual(2);
				}
			}
		}

		internal class when_running_the_tear_down_stage : SpecsFor<SpecsForEngine<IDisposable>>
		{
			protected override void When()
			{
				SUT.SUT = GetMockFor<IDisposable>().Object;
				SUT.TearDown();
			}

			[Test]
			public void then_it_runs_the_after_spec_callback()
			{
				GetMockFor<ISpecs<IDisposable>>()
					.Verify(s => s.AfterSpec());
			}

			[Test]
			public void then_it_runs_the_after_spec_behaviors()
			{
				GetMockFor<IBehaviorStack>()
					.Verify(s => s.ApplyAfterSpecTo(It.IsAny<ISpecs<IDisposable>>()));
			}

			[Test]
			public void then_it_disposes_the_system_under_test()
			{
				GetMockFor<IDisposable>()
					.Verify(d => d.Dispose());
			}
		}
	}
}