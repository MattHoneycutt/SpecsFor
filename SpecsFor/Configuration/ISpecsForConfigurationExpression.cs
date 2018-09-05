using System;

namespace SpecsFor.Core.Configuration
{
	public interface ISpecsForConfigurationExpression<T> where T : class
	{
		void EnrichWith<TEnricher>() where TEnricher : Behavior<T>, new();

		void CreateClassUnderTestUsing(Func<object> initializer);
	}
}