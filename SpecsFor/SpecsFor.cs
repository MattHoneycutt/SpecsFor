using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using StructureMap;
using StructureMap.AutoMocking;

namespace SpecsFor
{
	[TestFixture]
	public abstract class SpecsFor<T> : ITestState<T> where T: class
	{
		protected MoqAutoMocker<T> Mocker;
		protected List<IContext<T>> Contexts = new List<IContext<T>>();

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

		[TestFixtureSetUp]
		public virtual void SetupEachSpec() 
		{
			Mocker = new MoqAutoMocker<T>();

			ConfigureContainer(Mocker.Container);

			InitializeClassUnderTest();

			Given();

			When();
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
				if (SUT != null && SUT is IDisposable)
				{
					((IDisposable)SUT).Dispose();
				}				
			}
		}

		protected virtual void Given()
		{
			Contexts.ForEach(c => c.Initialize(this));
		}

		protected virtual void When()
		{

		}

		protected virtual void AfterEachSpec()
		{
			
		}

		protected void Given<TContext>() where TContext : IContext<T>, new()
		{
			Contexts.Add(new TContext());
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