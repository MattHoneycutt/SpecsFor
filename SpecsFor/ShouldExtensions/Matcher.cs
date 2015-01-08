using System;
using System.Linq.Expressions;

namespace SpecsFor.ShouldExtensions
{
	public class Matcher
	{
		[ThreadStatic]
		public static Matcher LastMatcher;

		public static void Create<T>(Expression<Func<T, bool>> matcher = null, string message = null)
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
			_message = message ?? "Object matching " + _matcher.Body;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is T))
			{
				return false;
			}

			var matcher = _matcher.Compile();

			return matcher((T)obj);
		}

		public override string ToString()
		{
			return _message;
		}
	}
}