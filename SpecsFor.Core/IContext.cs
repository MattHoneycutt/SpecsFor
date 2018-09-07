namespace SpecsFor.Core
{
	public interface IContext
	{
		void Initialize(ISpecs state);
	}

	public interface IContext<T>
	{
		void Initialize(ISpecs<T> state);
	}
}