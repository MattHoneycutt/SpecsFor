using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Moq;
using NUnit.Framework;
using SpecsFor.Configuration.Model;
using SpecsFor.Validation;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace SpecsFor
{
	[TestFixture]
	public abstract class SpecsForBase<T> : ISpecs<T> where T : class
	{
		private readonly SpecsForEngine<T> _engine;

        public T SUT
	    {
	        get => _engine.SUT;
	        set => _engine.SUT = value;
	    }

		protected SpecsForBase()
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
            return _engine.Mocker.GetMockFor<TMock>();
        }

	    protected abstract IAutoMocker CreateAutoMocker();
        
        public IAutoMocker Mocker => _engine.Mocker;

        [OneTimeSetUp]
	    public virtual void SetupEachSpec()
	    {
	        _engine.Init();

	        _engine.Given();

	        _engine.When();
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

        [OneTimeTearDown, Obsolete("This method will no longer be exposed for overriding.  Override AfterSpec instead.")]
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

        Mock<TMock> ISpecs.GetMockFor<TMock>()
        {
            return Mocker.GetMockFor<TMock>();
        }

        IAutoMocker ISpecs.CreateAutoMocker()
        {
            return CreateAutoMocker();
        }

        void ISpecs.Given()
		{
			Given();
		}

		void ISpecs.When()
		{
			When();
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