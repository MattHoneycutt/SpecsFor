using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SpecsFor.Configuration.Model;

namespace SpecsFor.Configuration
{
	public abstract class SpecsForConfiguration
	{
		private readonly List<ConditionalBehaviorAdapter> _behaviors = new List<ConditionalBehaviorAdapter>();

		//TODO: Need some way to identify when derived classes are not decorated with the SetupFixture attribute.

		[SetUp]
		public void ApplyConfiguration()
		{
			BehaviorStack.Push(this);
		}

		[TearDown]
		public void RemoveConfiguration()
		{
			BehaviorStack.Pop();
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
			var behavior = new TBehavior();

			_behaviors.Add(new ConditionalBehaviorAdapter(predicate, o => behavior.Given(((TSpec)o)), o => behavior.AfterSpec((TSpec)o)));
		}

		internal IEnumerable<ConditionalBehaviorAdapter> GetBehaviorsFor(Type targetType)
		{
			return _behaviors.Where(behavior => behavior.CanBeAppliedTo(targetType));
		}
	}
}