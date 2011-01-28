using System;
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

		[SetUp]
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

		[TearDown]
		public virtual void TearDown()
		{
			AfterEachSpec();
		}

		protected virtual void Given()
		{

		}

		protected virtual void AfterEachSpec()
		{
			
		}

		protected abstract void When();

		//TODO: Might not keep this...
		protected void Given<TContext>() where TContext : IContext<T>, new()
		{
			(new TContext()).Initialize(this);
		}

		protected void Given(Type[] context)
		{
		}
	}

	//These interfaces are part of SpecsFor V2 prototyping and may/may not survive 
	public interface IContext<T>
	{
		void Initialize(ITestState<T> state);
	}

	public interface ITestState<T>
	{
		T SUT { get; set; }
		Mock<TMock> GetMockFor<TMock>() where TMock : class;
	}

}