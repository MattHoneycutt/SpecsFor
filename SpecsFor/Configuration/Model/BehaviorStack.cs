using System;
using System.Collections.Generic;
using System.Linq;

namespace SpecsFor.Core.Configuration.Model
{
	internal class BehaviorStack : IBehaviorStack
	{
		[ThreadStatic]
		private static BehaviorStack _current;

		private readonly Stack<SpecsForConfiguration> _stack = new Stack<SpecsForConfiguration>();

		private static readonly BehaviorStack Empty = new BehaviorStack();

		public static BehaviorStack Current
		{
			get { return _current ?? Empty; }
		}

		private BehaviorStack()
		{
			//Prevents the class from being instantiated from outside.
		}

		public static void Push(SpecsForConfiguration config)
		{
			if (_current == null)
			{
				_current = new BehaviorStack();
			}

			_current._stack.Push(config);
		}

		public static void Pop()
		{
			if (_current == null)
			{
				throw new InvalidOperationException("Pop called without a corresponding Push call.  Call Push to push configuration on to the stack first.");
			}

			_current._stack.Pop();
		}

		private IEnumerable<IConditionalBehavior> FindBehaviorsFor(object specs)
		{
			var behaviors = _stack.Reverse().SelectMany(c => c.GetBehaviorsFor(specs.GetType()));
			return behaviors;
		}

		public void ApplyGivenTo(ISpecs specs)
		{
			var behaviors = FindBehaviorsFor(specs);

			foreach (var behavior in behaviors)
			{
				behavior.ApplyGivenTo(specs);
			}
		}

		public void ApplyAfterSpecTo(ISpecs specs)
		{
			var behaviors = FindBehaviorsFor(specs);

			foreach (var behavior in behaviors)
			{
				behavior.ApplyAfterSpecTo(specs);
			}
		}

		public void ApplySpecInitTo(ISpecs specs)
		{
			var behaviors = FindBehaviorsFor(specs);

			foreach (var behavior in behaviors)
			{
				behavior.ApplySpecInitTo(specs);
			}
		}

		public void ApplyAfterClassUnderTestInitializedTo(ISpecs specs)
		{
			var behaviors = FindBehaviorsFor(specs);

			foreach (var behavior in behaviors)
			{
				behavior.ApplyAfterClassUnderTestInitializedTo(specs);
			}
		}

		public Func<object> GetInitializationMethodFor(ISpecs specs)
		{
			return _stack.Reverse().Select(c => c.GetInitializationMethodFor(specs.GetType())).FirstOrDefault(i => i != null);
		}
	}
}