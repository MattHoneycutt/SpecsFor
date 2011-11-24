namespace SpecsFor.Demo.Domain
{
	public interface IPublisher
	{
		void Publish<TEvent>(TEvent @event);
	}
}