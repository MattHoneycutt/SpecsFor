using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SpecsFor.Configuration.Model;

namespace SpecsFor.Configuration
{
	public abstract class SpecsForConfiguration
	{
		private readonly List<IConditionalBehavior> _behaviors = new List<IConditionalBehavior>();
		private readonly List<ConditionalInitializer> _initializers = new List<ConditionalInitializer>();

		protected virtual void BeforeConfigurationApplied()
		{
			return;
		}

		[OneTimeSetUp]
		public virtual void ApplyConfiguration()
		{
			BeforeConfigurationApplied();
			BehaviorStack.Push(this);
			AfterConfigurationApplied();
		}

		protected virtual void AfterConfigurationApplied()
		{
			return;
		}

		protected virtual void BeforeConfigurationRemoved()
		{
			return;
		}

		[OneTimeTearDown]
		public virtual void RemoveConfiguration()
		{
			BeforeConfigurationRemoved();
			BehaviorStack.Pop();
			AfterConfigurationRemoved();
		}

		protected virtual void AfterConfigurationRemoved()
		{
			return;
		}

		protected ISpecsForConfigurationExpression<T> WhenTesting<T>() where T : class
		{
			return new SpecsForConfigurationExpression<T>(this, t => typeof (T).IsAssignableFrom(t));
		}

		protected ISpecsForConfigurationExpression<ISpecs> WhenTestingAnything()
		{
			return new SpecsForConfigurationExpression<ISpecs>(this, t => true);
		}

		protected ISpecsForConfigurationExpression<ISpecs> WhenTesting(Func<Type, bool> predicate)
		{
			return new SpecsForConfigurationExpression<ISpecs>(this, predicate);
		}

		internal void AddBehavior<TSpec, TBehavior>(Func<Type, bool> predicate)
			where TBehavior : Behavior<TSpec>, new()
			where TSpec : class
		{
			_behaviors.Add(new ConditionalBehavior<TSpec>(predicate, new TBehavior()));
		}

		internal IEnumerable<IConditionalBehavior> GetBehaviorsFor(Type targetType)
		{
			return _behaviors.Where(behavior => behavior.CanBeAppliedTo(targetType));
		}

		public Func<object> GetInitializationMethodFor(Type targetType)
		{
			var adapter = _initializers.FirstOrDefault(i => i.CanCreate(targetType));

			return adapter != null ? adapter.Initializer() : null;
		}

		internal void AddInitializer(Func<Type, bool> predicate, Func<object> initializer)
		{
			var adapter = new ConditionalInitializer
				(
				predicate,
				initializer
				);

			_initializers.Add(adapter);
		}
	}
}