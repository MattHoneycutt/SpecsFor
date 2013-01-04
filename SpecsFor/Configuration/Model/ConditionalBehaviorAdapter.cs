using System;

namespace SpecsFor.Configuration.Model
{
	internal class ConditionalBehaviorAdapter
	{
		private readonly Func<Type, bool> _predicate;
		private readonly Action<object> _applyGiven;
		private readonly Action<object> _applyAfterSpec;

		public ConditionalBehaviorAdapter(Func<Type, bool> predicate, Action<object> applyGiven, Action<object> applyAfterSpec)
		{
			_predicate = predicate;
			_applyGiven = applyGiven;
			_applyAfterSpec = applyAfterSpec;
		}

		public bool CanBeAppliedTo(Type targetType)
		{
			return _predicate(targetType);
		}

		public void ApplyGivenTo(object specs)
		{
			_applyGiven(specs);
		}

		public void ApplyAfterSpecTo(object specs)
		{
			_applyAfterSpec(specs);
		}
	}
}