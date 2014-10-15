using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using Should;
using Should.Core.Exceptions;

namespace SpecsFor.Helpers.Web.Mvc
{
	public static class RouteTestingHelpers
	{
		private static object GetValue(this RouteValueDictionary routeValues, string key)
		{
			var value = (from routeValueKey in routeValues.Keys
						 where string.Equals(routeValueKey, key, StringComparison.InvariantCultureIgnoreCase)
						 select routeValues[routeValueKey]).FirstOrDefault();

			value = value == null ? null : value.ToString();

			return value;
		}

		public static void ShouldMapTo<TController>(this RouteData routeData, Expression<Action<TController>> action)
			where TController : Controller
		{
			//check controller
			var expected = typeof(TController).Name.Replace("Controller", "");
			var actual = (string)routeData.Values.GetValue("controller");
			expected.ShouldEqual(actual, StringComparer.InvariantCultureIgnoreCase);

			//check action
			var methodCall = (MethodCallExpression)action.Body;
			var expectedAction = methodCall.Method.Name;
			var actualAction = routeData.Values.GetValue("action").ToString();
			expectedAction.ShouldEqual(actualAction, StringComparer.InvariantCultureIgnoreCase);

			//check parameters
			for (var i = 0; i < methodCall.Arguments.Count; i++)
			{
				var name = methodCall.Method.GetParameters()[i].Name;
				object value = null;

				switch (methodCall.Arguments[i].NodeType)
				{
					case ExpressionType.Constant:
						value = ((ConstantExpression)methodCall.Arguments[i]).Value;
						break;

					case ExpressionType.MemberAccess:
						value = Expression.Lambda(methodCall.Arguments[i]).Compile().DynamicInvoke();
						break;

				}

				value = (value == null ? null : value.ToString());

				var routeValue = routeData.Values.GetValue(name);

				if (!Equals(value, routeValue))
				{
					var message = string.Format("Route parameter '{0}' does not match. Expected '{1}', found '{2}'", name, value, routeValue);
					throw new AssertException(message);
				}
			}
		}
	}
}