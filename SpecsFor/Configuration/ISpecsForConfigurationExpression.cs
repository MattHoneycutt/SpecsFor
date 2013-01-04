namespace SpecsFor.Configuration
{
	public interface ISpecsForConfigurationExpression<T> where T : class
	{
		void EnrichWith<TEnricher>() where TEnricher : Behavior<T>, new();
	}
}