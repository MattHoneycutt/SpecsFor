namespace SpecsFor
{
	public interface IContext<T>
	{
		void Initialize(ITestState<T> state);
	}
}