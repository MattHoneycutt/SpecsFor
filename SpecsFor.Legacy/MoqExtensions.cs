using System.Collections.Generic;
using System.Linq;
using Moq.Language;
using Moq.Language.Flow;

namespace SpecsFor
{
	public static class MoqExtensions
	{
		//Makes it a little cleaner to stub out something that returns an IQueryable using an array.
		public static IReturnsResult<TMock> Returns<TMock, TResult>(this IReturns<TMock, IQueryable<TResult>> returns, IEnumerable<TResult> value) where TMock : class
		{
			return returns.Returns(value.AsQueryable());
		}
	}
}