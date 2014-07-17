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

			ShouldMatch(actual, memberInitExpression);
		}

		private static void ShouldMatch(object actual, MemberInitExpression expression)
		{
			var expected = Expression.Lambda<Func<object>>(expression).Compile()();
			var type = actual.GetType();

			foreach (var memberBinding in expression.Bindings)
			{
				var actualValue = type.GetProperty(memberBinding.Member.Name).GetValue(actual, null);
				var expectedValue = type.GetProperty(memberBinding.Member.Name).GetValue(expected, null);

				var bindingAsAnotherExpression = memberBinding as MemberAssignment;

				if (bindingAsAnotherExpression != null &&
					bindingAsAnotherExpression.Expression.NodeType == ExpressionType.MemberInit)
				{
					ShouldMatch(actualValue, bindingAsAnotherExpression.Expression as MemberInitExpression);
				}
				else
				{
					actualValue.ShouldEqual(expectedValue);
				}
			}
		}
	}
}