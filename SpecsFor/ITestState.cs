using Moq;

namespace SpecsFor
{
	//Non-generic interface added to make it easier to write tests. 
	public interface ITestState
	{
		Mock<TMock> GetMockFor<TMock>() where TMock : class;
	}

	public interface ITestState<T> : ITestState
	{
		T SUT { get; set; }
	}
}