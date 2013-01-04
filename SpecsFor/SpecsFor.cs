using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SpecsFor.Configuration.Model;
using StructureMap;
using StructureMap.AutoMocking;

namespace SpecsFor
{
	//TODO: This type is getting fairly complex.  What about splitting the 
	//		mocking helper methods either into extension methods, a partial
	//		class, or pulling those up into a base class that SpecsFor<T>
	//		derives from?

	[TestFixture]
	public abstract class SpecsFor<T> : ISpecs<T> where T: class
	{
		protected MoqAutoMocker<T> Mocker;
		protected List<IContext<T>> Contexts = new List<IContext<T>>();

		public T SUT { get; set; }

		private void TryDisposeSUT()
		{
			var sut = SUT as IDisposable;
			if (sut != null)
			{
				sut.Dispose();
			}
		}

		/// <summary>
		/// Gets the mock for the specified type from the underlying container. 
		/// </summary>
		/// <typeparam name="TMock"></typeparam>
		/// <returns></returns>
		public Mock<TMock> GetMockFor<TMock>() where TMock : class
		{
			return Mock.Get(Mocker.Get<TMock>());
		}

		/// <summary>
		/// Creates an IEnumerable of mock objects of T, and returns the mock objects
		/// for configuration.  Calling this method twice with the same 'enumerableSize'
		/// will return the same set of mocks. 
		/// </summary>
		/// <typeparam name="TMock"></typeparam>
		/// <param name="enumerableSize"></param>
		/// <returns></returns>
		public Mock<TMock>[] GetMockForEnumerableOf<TMock>(int enumerableSize) where TMock : class
		{
			var existingMocks = Mocker.Container.Model.InstancesOf<TMock>().ToArray();

			if (existingMocks.Length > 0)
			{
				if (existingMocks.Length != enumerableSize)
				{
					throw new InvalidOperationException("An IEnumerable for this type of mock has already been created with size " +
					                                    existingMocks.Length + ".");
				}

				return Mocker.Container.GetAllInstances<TMock>().Select(Mock.Get).ToArray();
			}

			var mocks = Mocker.CreateMockArrayFor<TMock>(enumerableSize);

			return mocks.Select(Mock.Get).ToArray();
		}

		protected void Given<TContext>() where TContext : IContext<T>, new()
		{
			Given(new TContext());
		}

		protected void Given(IContext<T> context)
		{
			context.Initialize(this);
		}

		[TestFixtureSetUp]
		public virtual void SetupEachSpec() 
		{
			Mocker = new MoqAutoMocker<T>();

			ConfigureContainer(Mocker.Container);

			InitializeClassUnderTest();

			try
			{
				Given();

				BehaviorStack.Current.ApplyGivenTo(this);

				When();
			}
			catch (Exception)
			{
				TryDisposeSUT();
				throw;
			}
		}

		protected virtual void ConfigureContainer(IContainer container)
		{
		}

		protected virtual void InitializeClassUnderTest()
		{
			SUT = Mocker.ClassUnderTest;
		}

		protected virtual void Given()
		{
		}

		protected virtual void When()
		{
		}

		[TearDown]
		protected virtual void AfterEachTest()
		{
		}

		[TestFixtureTearDown]
		public virtual void TearDown()
		{
			try
			{
				AfterSpec();

				BehaviorStack.Current.ApplyAfterSpecTo(this);
			}
			finally
			{
				TryDisposeSUT();
			}
		}

		protected virtual void AfterSpec()
		{
		}
	}
}