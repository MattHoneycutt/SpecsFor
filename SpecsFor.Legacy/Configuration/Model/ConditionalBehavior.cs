using System;

namespace SpecsFor.Configuration.Model
{
	internal class ConditionalBehavior<TSpec> : IConditionalBehavior where TSpec : class
	{
		private readonly Func<Type, bool> _predicate;
		private readonly Behavior<TSpec> _behavior;

		public ConditionalBehavior(Func<Type, bool> predicate, Behavior<TSpec> behavior)
		{
			_predicate = predicate;
			_behavior = behavior;
		}

		public bool CanBeAppliedTo(Type targetType)
		{
			return _predicate(targetType);
		}

		public void ApplyGivenTo(object specs)
		{
			_behavior.Given((TSpec) specs);
		}

		public void ApplyAfterSpecTo(object specs)
		{
			_behavior.AfterSpec((TSpec)specs);
		}

		public void ApplySpecInitTo(object specs)
		{
			_behavior.SpecInit((TSpec)specs);
		}

		public void ApplyAfterClassUnderTestInitializedTo(object specs)
		{
			_behavior.ClassUnderTestInitialized((TSpec)specs);
		}
	}
}