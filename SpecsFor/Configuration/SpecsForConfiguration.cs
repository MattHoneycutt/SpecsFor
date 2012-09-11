using System;
using System.Collections;
using System.Collections.Generic;
using StructureMap;
using System.Linq;

namespace SpecsFor.Configuration
{
	public class SpecsForConfiguration
	{
		private readonly List<ConditionalBehavior> _behaviors = new List<ConditionalBehavior>();
		//internal Container Container { get; private set; }

		public SpecsForConfiguration()
		{
			//Container = new Container();
		}

		public ISpecsForConfigurationExpression<T> WhenTesting<T>() where T : class
		{
			return new SpecsForConfigurationExpression<T>(this, t => typeof (T).IsAssignableFrom(t));
		}

		internal void AddBehavior<TSpec, TBehavior>(Func<Type, bool> predicate) where TBehavior : IBehavior<TSpec>, new() where TSpec : class
		{
			_behaviors.Add(new ConditionalBehavior(predicate, (object o) => new TBehavior().Given(((TSpec)o))));
		}

		//Would be nice to have a base, non-generic ISpecsFor interface or something perhaps...
		public ISpecsForConfigurationExpression<object> WhenTestingAnything()
		{
			return new SpecsForConfigurationExpression<object>(this, t => true);
		}

		public ISpecsForConfigurationExpression<object> WhenTesting(Func<Type, bool> predicate)
		{
			return new SpecsForConfigurationExpression<object>(this, predicate);
		}

		internal IEnumerable<ConditionalBehavior> GetBehaviorsFor(Type targetType)
		{
			return _behaviors.Where(behavior => behavior.CanBeAppliedTo(targetType));
		}
	}

	internal class ConditionalBehavior
	{
		private readonly Func<Type, bool> _predicate;
		private readonly Action<object> _applyGiven;

		public ConditionalBehavior(Func<Type, bool> predicate, Action<object> applyGiven)
		{
			_predicate = predicate;
			_applyGiven = applyGiven;
		}

		public bool CanBeAppliedTo(Type targetType)
		{
			return _predicate(targetType);
		}

		public void ApplyTo(object specs)
		{
			_applyGiven(specs);
		}
	}

	public interface IBehavior<T> where T : class
	{
		void Given(T instance);
	}

	public static class SpecsForBehaviors
	{
		[ThreadStatic]
		private static SpecsForConfiguration _current = new SpecsForConfiguration();

		public static void Configure(Action<SpecsForConfiguration> cfgAction)
		{
			cfgAction(_current);
		}

		public static void ApplyBehaviorsFor(object specs)
		{
			var behaviors = _current.GetBehaviorsFor(specs.GetType());

			foreach (var behavior in behaviors)
			{
				behavior.ApplyTo(specs);
			}
		}

		private static IEnumerable<Type> GetAllBaseTypes(Type rootType)
		{
			var current = rootType.BaseType;

			while (current != null)
			{
				yield return current;
				current = current.BaseType;
			}
		}
	}

	public class SpecsForConfigurationExpression<TSpec> : ISpecsForConfigurationExpression<TSpec> where TSpec : class
	{
		private readonly SpecsForConfiguration _config;
		private readonly Func<Type, bool> _predicate;

		public SpecsForConfigurationExpression(SpecsForConfiguration config, Func<Type, bool> predicate)
		{
			_config = config;
			_predicate = predicate;
		}

		public void EnrichWith<TEnricher>() where TEnricher : IBehavior<TSpec>, new()
		{
			_config.AddBehavior<TSpec, TEnricher>(_predicate);
		}
	}

	public interface ISpecsForConfigurationExpression<T> where T : class
	{
		void EnrichWith<TEnricher>() where TEnricher : IBehavior<T>, new();
	}

}