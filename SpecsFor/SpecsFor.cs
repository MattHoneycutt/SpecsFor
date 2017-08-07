﻿using JetBrains.Annotations;
using Moq;
using NUnit.Framework;
using SpecsFor.AutoMocking;
using SpecsFor.Configuration.Model;
using SpecsFor.Validation;
using StructureMap;

namespace SpecsFor
{
	[TestFixture]
	public abstract class SpecsFor<T> : ISpecs<T> where T : class
	{
		private readonly SpecsForEngine<T> _engine;

		public IContainer MockContainer
		{
			get { return _engine.Mocker.Container; }
		}

		public T SUT { get { return _engine.SUT; } set { _engine.SUT = value; } }

		protected SpecsFor()
		{
			_engine = new SpecsForEngine<T>(this, BehaviorStack.Current, new NUnitSpecValidator());
		}

	    /// <summary>
		/// Gets the mock for the specified type from the underlying container. 
		/// </summary>
		/// <typeparam name="TMock"></typeparam>
		/// <returns></returns>
		public Mock<TMock> GetMockFor<TMock>() where TMock : class
		{
			return _engine.Mocker.GetMockFor<T, TMock>();
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
			return _engine.Mocker.GetMockForEnumerableOf<T, TMock>(enumerableSize);	
		}

	    [UsedImplicitly]
        public MoqAutoMocker<T> Mocker => _engine.Mocker;

	    [OneTimeSetUp]
	    public virtual void SetupEachSpec()
	    {
	        _engine.Init();

	        _engine.Given();

	        _engine.When();
	    }

	    protected virtual void ConfigureContainer(IContainer container)
	    {
	    }

	    [UsedImplicitly]
	    protected void Given<TContext>() where TContext : IContext<T>, new()
		{
			_engine.ApplyContext(new TContext());
		}

	    protected void Given(IContext<T> context)
		{
			_engine.ApplyContext(context);
		}

	    [UsedImplicitly]
        protected void Given(IContext context)
		{
			_engine.ApplyContext(context);
		}

	    protected virtual void InitializeClassUnderTest()
		{
			_engine.InitializeClassUnderTest();
		}

		protected virtual void Given()
		{
		}

		protected virtual void When()
		{
		}

        /// <summary>
        /// Runs before each individual test case.  Use carefully!
        /// </summary>
        [SetUp]
	    protected virtual void BeforeEachTest()
	    {
	        
	    }

        /// <summary>
        /// Runs after each individual test case.  Use carefully!
        /// </summary>
        [TearDown]
		protected virtual void AfterEachTest()
		{
		}

        /// <summary>
        /// Runs when the entire suite of specs is complete.  If you override this,
        /// be sure to call the base implementation, otherwise your specs will not 
        /// be cleaned up correctly!
        /// </summary>
		[OneTimeTearDown]
		public virtual void TearDown()
		{
			_engine.TearDown();
		}

        /// <summary>
        /// Runs after the entire suite of specs is complete.  You can safely hook
        /// in here to do any last-minute cleanup. 
        /// </summary>
		protected virtual void AfterSpec()
		{
		}

		#region ISpecs implementation

		void ISpecs.Given()
		{
			Given();
		}

		void ISpecs.When()
		{
			When();
		}

		void ISpecs.ConfigureContainer(IContainer container)
		{
			ConfigureContainer(container);
		}

		void ISpecs.InitializeClassUnderTest()
		{
			InitializeClassUnderTest();
		}

		void ISpecs.AfterSpec()
		{
			AfterSpec();
		}

		#endregion
	}
}