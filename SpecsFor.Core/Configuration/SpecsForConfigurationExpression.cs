using System;

namespace SpecsFor.Core.Configuration
{
	public class SpecsForConfigurationExpression<TSpec> : ISpecsForConfigurationExpression<TSpec> where TSpec : class
	{
		private readonly SpecsForConfiguration _config;
		private readonly Func<Type, bool> _predicate;

		public SpecsForConfigurationExpression(SpecsForConfiguration config, Func<Type, bool> predicate)
		{
			_config = config;
			_predicate = predicate;
		}

		public void EnrichWith<TEnricher>() where TEnricher : Behavior<TSpec>, new()
		{
			_config.AddBehavior<TSpec, TEnricher>(_predicate);
		}

		//TODO: Need some tests.
		public void CreateClassUnderTestUsing(Func<object> initializer)
		{
			_config.AddInitializer(_predicate, initializer);
		}
	}
}