using Moq;

namespace SpecsFor
{
	public interface ISpecs
	{
		Mock<TMock> GetMockFor<TMock>() where TMock : class;

		/// <summary>
		/// Creates an IEnumerable of mock objects of T, and returns the mock objects
		/// for configuration.  Calling this method twice with the same 'enumerableSize'
		/// will return the same set of mocks. 
		/// </summary>
		/// <typeparam name="TMock"></typeparam>
		/// <param name="enumerableSize"></param>
		/// <returns></returns>
		Mock<TMock>[] GetMockForEnumerableOf<TMock>(int enumerableSize) where TMock : class;
	}

	public interface ISpecs<T> : ISpecs
	{
		T SUT { get; set; }
	}
}