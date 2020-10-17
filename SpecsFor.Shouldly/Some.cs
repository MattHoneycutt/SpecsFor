using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SpecsFor.Shouldly
{
	public static class Some
	{
		public static TimeSpan DefaultDateTimeTolerance = TimeSpan.FromSeconds(1);

		public static T ValueOf<T>(Expression<Func<T, bool>> matcher)
		{
			Matcher.Create(matcher, "Object matching " + matcher.Body);

			return default(T);
		}

		public static T ValueInRange<T>(T min, T max) where T : IComparable
		{
			return ValueInRange(min, max, true);
		}

		public static T ValueInRange<T>(T min, T max, bool inclusive) where T: IComparable
		{
			if (inclusive)
			{
				var message = string.Format("Object greater than or equal to {0} and less than or equal to {1}", min, max);
				Matcher.Create<T>(x => x.CompareTo(min) >= 0 && x.CompareTo(max) <= 0, message);
			}
			else
			{
				var message = string.Format("Object greater than {0} and less than {1}", min, max);
				Matcher.Create<T>(x => x.CompareTo(min) > 0 && x.CompareTo(max) < 0, message);				
			}

			return default(T);
		}

		public static DateTime DateTimeNear(DateTime value)
		{
			return DateTimeNear(value, null);
		}

		public static DateTime DateTimeNear(DateTime value, TimeSpan? tolerance)
		{
			var actualTolerance = tolerance ?? DefaultDateTimeTolerance;

			return ValueInRange(value.Subtract(actualTolerance), value.Add(actualTolerance));
		}

		public static DateTimeOffset DateTimeNear(DateTimeOffset value)
		{
			return DateTimeNear(value, null);
		}

		public static DateTimeOffset DateTimeNear(DateTimeOffset value, TimeSpan? tolerance)
		{
			var actualTolerance = tolerance ?? DefaultDateTimeTolerance;

			return ValueInRange(value.Subtract(actualTolerance), value.Add(actualTolerance));
		}

	    public static T[] ListContaining<T>(Expression<Func<T>> initializer) where T : class
	    {
	        Matcher.Create<IEnumerable<T>>(x => x.ContainsMatch(initializer), $"Expected list containing item matching [{initializer.Body}], but match was not found.");

	        return default(T[]);
	    }

	    public static T[] ListContaining<T>(T obj)
	    {
	        Matcher.Create<IEnumerable<T>>(x => x.Contains(obj), $"Expected list containing [{obj}], but item was not found.");

	        return default(T[]);
	    }
    }
}