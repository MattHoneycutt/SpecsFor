using Moq;

namespace SpecsFor.Core
{
	public interface ISpecs
	{
		Mock<TMock> GetMockFor<TMock>() where TMock : class;

	    IAutoMocker CreateAutoMocker();

	    IAutoMocker Mocker { get; }

        void InitializeClassUnderTest();

		void Given();

		void When();

		void AfterSpec();
		
		void AfterTest();

		void BeforeTest();
	}

	public interface ISpecs<T> : ISpecs
    {
		T SUT { get; set; }
    }
}