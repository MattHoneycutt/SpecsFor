namespace SpecsFor.Demo.Domain
{
	public interface IValidator<T>
	{
		bool Validate(T obj);
	}
}