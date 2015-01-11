using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Should;
using Should.Core.Exceptions;

namespace SpecsFor.ShouldExtensions
{
	public static class ShouldLooksLikeExtensions
	{
		public static void ShouldLookLike<T>(this T actual, Expression<Func<T>> matchFunc) where T : class
		{
			var memberInitExpression = matchFunc.Body as MemberInitExpression;
			var newArrayExpression = matchFunc.Body as NewArrayExpression;

			if (memberInitExpression != null)
			{
				ShouldMatch(actual, memberInitExpression);
			}
			else if (newArrayExpression != null)
			{
				var actualAsIEnumerable = actual as IEnumerable;

				if (actualAsIEnumerable == null)
				{
					throw new InvalidOperationException("Actual value isn't IEnumerable, yet expression is.");
				}

				ShouldMatchIEnumerable(actualAsIEnumerable, newArrayExpression);
			}
			else
			{
				throw new InvalidOperationException(
					"You must pass in an initialization expression, such as 'new SomeObject{..}' or 'new[] { new SomeObject{...}, new SomeObject{...}'");
			}
		}

		private static void ShouldMatchIEnumerable(IEnumerable actual, NewArrayExpression arrayExpression)
		{
			var array = actual.Cast<object>().ToArray();

			if (arrayExpression.Expressions.Any(x => !(x is MemberInitExpression)))
			{
				var expected = (IEnumerable)Expression.Lambda<Func<object>>(arrayExpression).Compile()();
				var expectedArray = expected.Cast<object>().ToArray();

				array.ShouldLookLike(expectedArray); 
				
				return;
			}

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
					ShouldMatchIEnumerable(actualValue as IEnumerable, bindingAsAnotherExpression.Expression as NewArrayExpression);
				}
				else if (IsMoqExpression(bindingAsAnotherExpression))
				{
					Assert.Fail("Moq's matchers cannot be used with SpecsFor's partial matching.  Instead, use the Some.Of and Any.Of methods in SpecsFor.");
				}
				else if (IsSpecsForAnyExpression(bindingAsAnotherExpression))
				{
					var expectedExpression = TryUnwrapCallExpression(bindingAsAnotherExpression);

					//Re-invoke the expression.  This is needed so that it will be the last matcher on the stack.
					Matcher.LastMatcher = null;
					Expression expressionAsObject = Expression.Convert(expectedExpression, typeof (object));
					Expression.Lambda<Func<object>>(expressionAsObject).Compile()();

					if (Matcher.LastMatcher == null ||
					    !Matcher.LastMatcher.Equals(actualValue))
					{
						throw new EqualException(Matcher.LastMatcher.ToString(), actualValue);
					}
				}
				else
				{
					actualValue.ShouldEqual(expectedValue);
				}
			}
		}

		private static bool IsMoqExpression(MemberAssignment bindingAsAnotherExpression)
		{
			if (bindingAsAnotherExpression == null) return false;

			var callExpression = TryUnwrapCallExpression(bindingAsAnotherExpression);

			if (callExpression == null || callExpression.Method.DeclaringType != typeof(It)) return false;

			return true;
		}

		private static bool IsSpecsForAnyExpression(MemberAssignment bindingAsAnotherExpression)
		{
			if (bindingAsAnotherExpression == null) return false;

			var callExpression = TryUnwrapCallExpression(bindingAsAnotherExpression);

			if (callExpression == null) return false;

			if (callExpression.Method.DeclaringType != typeof(Any) &&
				callExpression.Method.DeclaringType != typeof(Some)) return false;

			return true;
		}

		private static MethodCallExpression TryUnwrapCallExpression(MemberAssignment bindingAsAnotherExpression)
		{
			var targetExpression = bindingAsAnotherExpression.Expression;

			if (targetExpression.NodeType == ExpressionType.Convert)
			{
				var convertExpression = (UnaryExpression)targetExpression;
				targetExpression = convertExpression.Operand;
			}

			if (targetExpression.NodeType != ExpressionType.Call) return null;

			return (MethodCallExpression)targetExpression;
		}
	}
}