using System;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Should;

namespace SpecsFor.ShouldExtensions
{
	public static class ShouldLooksLikeExtensions
	{
		public static void ShouldLookLike<T>(this T actual, Expression<Func<T>> matchFunc) where T : class
		{
			var memberInitExpression = (matchFunc.Body as MemberInitExpression);

			if (matchFunc.Body.NodeType != ExpressionType.MemberInit || memberInitExpression == null)
			{
				Assert.Fail("The matching expression can only be a new object declaration.");
			}

			var expected = matchFunc.Compile()();

			foreach (var memberBinding in memberInitExpression.Bindings)
			{
				var actualValue = typeof(T).GetProperty(memberBinding.Member.Name).GetValue(actual, null);
				var expectedValue = typeof(T).GetProperty(memberBinding.Member.Name).GetValue(expected, null);

				actualValue.ShouldEqual(expectedValue);
			}
		}
	}
}