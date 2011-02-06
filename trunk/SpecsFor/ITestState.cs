using Moq;

namespace SpecsFor
{
	public interface ITestState<T>
	{
		T SUT { get; set; }
		Mock<TMock> GetMockFor<TMock>() where TMock : class;
	}
}