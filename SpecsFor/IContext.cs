namespace SpecsFor
{
	public interface IContext<T>
	{
		void Initialize(ISpecs<T> state);
	}
}