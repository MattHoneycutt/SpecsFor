using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace SpecsFor.Core.ShouldExtensions
{
	public static class ContainsExtensions
	{
		public static void ShouldContain<T>(this IEnumerable<T> list, Expression<Func<T, bool>> filter)
		{
			if (list.Count(filter.Compile()) == 0)
			{
				Assert.Fail("Expected list containing item matching {0}, but item was not found.", filter);
			}
		}

		public static void ShouldNotContain<T>(this IEnumerable<T> list, Expression<Func<T, bool>> filter)
		{
			if (list.Any(filter.Compile()))
			{
				Assert.Fail("Expected list not containing item matching {0}, but found a matching item.", filter);
			}
		}

	    public static bool ContainsMatch<T>(this IEnumerable<T> list, Expression<Func<T>> initializer) where T : class
	    {
	        if (list == null) return false;

	        foreach (var item in list)
	        {
	            if (item.LooksLike(initializer))
	                return true;
	        }

	        return false;
	    }
	}
}