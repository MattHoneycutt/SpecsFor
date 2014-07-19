using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
			var memberInitExpression = matchFunc.Body as MemberInitExpression;

			if (matchFunc.Body.NodeType == ExpressionType.MemberInit || memberInitExpression != null)
			{
				ShouldMatch(actual, memberInitExpression);
			}

			var newArrayExpression = matchFunc.Body as NewArrayExpression;

			if (matchFunc.Body.NodeType == ExpressionType.NewArrayInit || newArrayExpression != null)
			{
				var actualAsIEnumerable = actual as IEnumerable<object>;

				if (actualAsIEnumerable == null)
				{
					throw new InvalidOperationException("Actual value isn't IEnumerable, yet expression is.");
				}

				ShouldMatchIEnumerable(actualAsIEnumerable, newArrayExpression);
			}

		}

		private static void ShouldMatchIEnumerable(IEnumerable<object> actual, NewArrayExpression arrayExpression)
		{
			var array = actual.ToArray();
			for (int i = 0; i < arrayExpression.Expressions.Count; i++)
			{
				ShouldMatch(array[i], arrayExpression.Expressions[i] as MemberInitExpression);
			}
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
				else if (bindingAsAnotherExpression != null &&
						bindingAsAnotherExpression.Expression.NodeType == ExpressionType.NewArrayInit)
				{
					ShouldMatchIEnumerable(actualValue as IEnumerable<object>, bindingAsAnotherExpression.Expression as NewArrayExpression);
				}
				else 
				{
					actualValue.ShouldEqual(expectedValue);
				}
			}
		}
	}
}