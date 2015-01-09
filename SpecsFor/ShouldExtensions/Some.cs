using System;
using System.Linq.Expressions;

namespace SpecsFor.ShouldExtensions
{
	public static class Some
	{
		public static T ValueOf<T>(Expression<Func<T, bool>> matcher)
		{
			Matcher.Create(matcher, "Object matching " + matcher.Body);

			return default(T);
		}
	}
}