using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SpecsFor.Configuration;
using StructureMap;
using StructureMap.AutoMocking;

namespace SpecsFor
{
	[TestFixture]
	public abstract class SpecsFor<T> : ITestState<T> where T: class
	{
		protected MoqAutoMocker<T> Mocker;
		protected List<IContext<T>> Contexts = new List<IContext<T>>();

		private void TryDisposeSUT()
		{
			if (SUT != null && SUT is IDisposable)
			{
				((IDisposable)SUT).Dispose();
			}
		}

		//TODO: Obsolete this as well.
		protected TContextType GetContext<TContextType>() where TContextType : IContext<T>
		{
			return (TContextType)Contexts.FirstOrDefault(c => c.GetType() == typeof(TContextType));
		}

		protected TContextType GetContext<TContextType>(Func<IEnumerable<TContextType>, TContextType> search) where TContextType : IContext<T>
		{
			return search((IEnumerable<TContextType>)Contexts.Where(c => c.GetType() == typeof(TContextType)));
		}

		protected SpecsFor()
		{
			
		}

		//And make this obsolete.
		protected SpecsFor(Type[] contexts)
		{
			Given(contexts);
		}

		public T SUT { get; set; }

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
		protected Mock<TMock>[] GetMockForEnumerableOf<TMock>(int enumerableSize) where TMock : class
		{
			var existingMocks = Mocker.Container.Model.InstancesOf<TMock>().ToArray();

			if (existingMocks.Length > 0)
			{
				if (existingMocks.Length != enumerableSize)
				{
					throw new InvalidOperationException("An IEnumerable for this type of mock has already been created with size " +
					                                    existingMocks.Length + ".");
				}

				return Mocker.Container.GetAllInstances<TMock>().Select(i => Mock.Get<TMock>(i)).ToArray();
			}

			var mocks = Mocker.CreateMockArrayFor<TMock>(enumerableSize);

			return mocks.Select(m => Mock.Get<TMock>(m)).ToArray();
		}

		[TestFixtureSetUp]
		public virtual void SetupEachSpec() 
		{
			Mocker = new MoqAutoMocker<T>();

			ConfigureContainer(Mocker.Container);

			InitializeClassUnderTest();

			try
			{
				//TODO: Do the standard Given stuff (such as applying Given<T> context) outside of the base Given.
				Given();

				SpecsForBehaviors.ApplyBehaviorsFor(this);

				When();
			}
			catch (Exception)
			{
				TryDisposeSUT();
				throw;
			}
		}

		protected virtual void InitializeClassUnderTest()
		{
			SUT = Mocker.ClassUnderTest;
		}

		protected virtual void ConfigureContainer(IContainer container)
		{
		}

		[TestFixtureTearDown]
		public virtual void TearDown()
		{
			try
			{
				AfterEachSpec();
			}
			finally
			{
				TryDisposeSUT();
			}
		}

		protected virtual void Given()
		{
			Contexts.ForEach(c => c.Initialize(this));
		}

		protected virtual void When()
		{

		}

		//TODO: Obsolete this and add a replacement. 
		protected virtual void AfterEachSpec()
		{
			
		}

		protected void Given<TContext>() where TContext : IContext<T>, new()
		{
			Given(new TContext());
		}

		protected void Given(IContext<T> context)
		{
			Contexts.Add(context);
		}

		protected void Given(Type[] context)
		{
			var contexts = (from c in context
			                select Activator.CreateInstance(c))
							.Cast<IContext<T>>();

			Contexts.AddRange(contexts);
		}
	}
}