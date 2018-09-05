using System;

namespace SpecsFor.Core.Configuration
{
	internal class ConditionalInitializer
	{
		private readonly Func<Type, bool> _predicate;
		private readonly Func<object> _initializer;

		public ConditionalInitializer(Func<Type, bool> predicate, Func<object> initializer)
		{
			_predicate = predicate;
			_initializer = initializer;
		}

		public bool CanCreate(Type targetType)
		{
			return _predicate(targetType);
		}

		public Func<object> Initializer()
		{
			return _initializer;
		}
	}
}