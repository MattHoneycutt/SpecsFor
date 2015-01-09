using System;
using System.Linq.Expressions;
using StructureMap.TypeRules;

namespace SpecsFor.ShouldExtensions
{
	public class Matcher
	{
		[ThreadStatic]
		public static Matcher LastMatcher;

		public static void Create<T>(Expression<Func<T, bool>> matcher, string message)
		{
			LastMatcher = new Matcher<T>(matcher, message);
		}
	}

	public class Matcher<T> : Matcher
	{
		private readonly Expression<Func<T, bool>> _matcher;
		private readonly string _message;

		public Matcher(Expression<Func<T, bool>> matcher, string message)
		{
			_matcher = matcher ?? (x => true);
			_message = message;
		}

		public override bool Equals(object obj)
		{
			if (!ObjectIsCompatibleWithType(obj))
			{
				return false;
			}

			var matcher = _matcher.Compile();

			return matcher((T)obj);
		}

		private static bool ObjectIsCompatibleWithType(object obj)
		{
			if (obj is T) return true;

			if (typeof (T).IsNullable() && (typeof (T).GetInnerTypeFromNullable()) == obj.GetType())
				return true;

			return false;
		}

		public override string ToString()
		{
			return _message;
		}
	}
}