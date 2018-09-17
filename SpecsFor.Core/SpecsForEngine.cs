using System;
using System.Collections.Generic;
using SpecsFor.Core.Configuration.Model;
using SpecsFor.Core.Validation;

namespace SpecsFor.Core
{
	internal class SpecsForEngine<T> where T : class
	{
		private readonly ISpecs<T> _specs;
		private readonly IBehaviorStack _currentBehaviors;
		private readonly ISpecValidator _validator;
		private readonly List<Exception> _exceptions = new List<Exception>();

        public IAutoMocker Mocker { get; set; }

		public T SUT { get; set; }

		public SpecsForEngine(ISpecs<T> specs, IBehaviorStack currentBehaviors, ISpecValidator validator)
		{
			_specs = specs;
			_currentBehaviors = currentBehaviors;
			_validator = validator;
		}

		public void ApplyContext(IContext<T> context)
		{
			context.Initialize(_specs);
		}

		public void ApplyContext(IContext context)
		{
			context.Initialize(_specs);
		}

		public void Init()
		{
			_validator.ValidateSpec(_specs);

			try
			{
			    Mocker = _specs.CreateAutoMocker();
				
				_currentBehaviors.ApplySpecInitTo(_specs);

			    Mocker.ConfigureContainer(_specs);

				_specs.InitializeClassUnderTest();

				_currentBehaviors.ApplyAfterClassUnderTestInitializedTo(_specs);
			}
			catch (Exception ex)
			{
				_exceptions.Add(ex);
				HandleError();
				throw new SpecInitException(_exceptions.ToArray());
			}
		}
		
		public void InitializeClassUnderTest()
		{
			var initializationMethod = _currentBehaviors.GetInitializationMethodFor(_specs);

			if (initializationMethod != null)
			{
				SUT = (T) initializationMethod();
			}
			else
			{
			   SUT = Mocker.CreateSUT<T>();
			}
		}

		public void Given()
		{
			try
			{
				_currentBehaviors.ApplyGivenTo(_specs);

				_specs.Given();
			}
			catch (Exception ex)
			{
				_exceptions.Add(ex);
				HandleError();
				TryDisposeSUT();
				throw new GivenSpecificationException(_exceptions.ToArray());
			}
		}

		public void When()
		{
			try
			{
				_specs.When();
			}
			catch (Exception ex)
			{
				_exceptions.Add(ex);
				HandleError();
				TryDisposeSUT();
				throw new WhenSpecificationException(_exceptions.ToArray());
			}
		}

		public void TearDown()
		{
			try
			{
				_specs.AfterSpec();

				_currentBehaviors.ApplyAfterSpecTo(_specs);
			}
			finally
			{
				TryDisposeSUT();
			}
		}

		private void TryDisposeSUT()
		{
		    if (!(SUT is IDisposable sut)) return;

			try
			{
				sut.Dispose();
			}
			catch (Exception ex)
			{
				_exceptions.Add(ex);
			}
		}

		private void HandleError()
		{
			try
			{
				_currentBehaviors.ApplyAfterSpecTo(_specs);
			}
			catch (Exception ex)
			{
				_exceptions.Add(ex);
			}
		}
	}
}