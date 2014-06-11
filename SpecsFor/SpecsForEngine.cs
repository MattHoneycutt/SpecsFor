using System;
using System.Collections.Generic;
using SpecsFor.Configuration.Model;
using SpecsFor.Validation;
using StructureMap.AutoMocking.Moq;

namespace SpecsFor
{
	internal class SpecsForEngine<T> where T : class
	{
		private readonly ISpecs<T> _specs;
		private readonly IBehaviorStack _currentBehaviors;
		private readonly ISpecValidator _validator;
		private readonly List<Exception> _exceptions = new List<Exception>();

		public MoqAutoMocker<T> Mocker { get; private set; }

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

		public void Init()
		{
			_validator.ValidateSpec(_specs);

			try
			{
				Mocker = new MoqAutoMocker<T>();

				_currentBehaviors.ApplySpecInitTo(_specs);

				_specs.ConfigureContainer(Mocker.Container);

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
				SUT = Mocker.ClassUnderTest;
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
			var sut = SUT as IDisposable;
			if (sut == null) return;

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