using Moq;
using NUnit.Framework;
using SpecsFor.Configuration.Model;
using SpecsFor.Validation;
using StructureMap;
using StructureMap.AutoMocking;
using StructureMap.AutoMocking.Moq;

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

		[TestFixtureSetUp]
		public virtual void SetupEachSpec()
		{
			_engine.Init();

			_engine.Given();

			_engine.When();
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

		public MoqAutoMocker<T> Mocker
		{
			get { return _engine.Mocker;}
		} 

		protected void Given<TContext>() where TContext : IContext<T>, new()
		{
			_engine.ApplyContext(new TContext());
		}

		protected void Given(IContext<T> context)
		{
			_engine.ApplyContext(context);
		}

		protected virtual void ConfigureContainer(IContainer container)
		{
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

		[TearDown]
		protected virtual void AfterEachTest()
		{
		}

		[TestFixtureTearDown]
		public virtual void TearDown()
		{
			_engine.TearDown();
		}

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