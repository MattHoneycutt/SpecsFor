using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace SpecsFor.Mvc.Helpers
{
	/// <summary>
	/// Used to simplify testing routes.  Adapted from MvcContrib. 
	/// </summary>
	public static class RouteTestingExtensions
	{
		public static void AssertSameStringAs(this string expected, string actual)
		{
			if (!string.Equals(expected, actual, StringComparison.InvariantCultureIgnoreCase))
			{
				var message = string.Format("Expected {0} but was {1}", expected, actual);
				throw new AssertionException(message);
			}
		}

		/// <summary>
		/// Asserts that the route matches the expression specified.  Checks controller, action, and any method arguments
		/// into the action as route values.
		/// </summary>
		/// <typeparam name="TController">The controller.</typeparam>
		/// <param name="routeData">The routeData to check</param>
		/// <param name="action">The action to call on TController.</param>
		public static RouteData ShouldMapTo<TController>(this RouteData routeData, Expression<Func<TController, ActionResult>> action)
			where TController : Controller
		{
			if (routeData == null)
			{
				throw new AssertionException("The URL did not match any route");
			}

			//check controller
			routeData.ShouldMapTo<TController>();

			//check action
			var methodCall = (MethodCallExpression)action.Body;
			string expectedAction = methodCall.Method.Name;
			string actualAction = routeData.Values.GetValue("action").ToString();
			expectedAction.AssertSameStringAs(actualAction);

			//check parameters
			for (int i = 0; i < methodCall.Arguments.Count; i++)
			{
				string name = methodCall.Method.GetParameters()[i].Name;
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

				if (!object.Equals(value, routeValue))
				{
					throw new RouteAssertionException(name, value, routeValue);
				}
			}

			return routeData;
		}

		///// <summary>
		///// Converts the URL to matching RouteData and verifies that it will match a route with the values specified by the expression.
		///// </summary>
		///// <typeparam name="TController">The type of controller</typeparam>
		///// <param name="relativeUrl">The ~/ based url</param>
		///// <param name="action">The expression that defines what action gets called (and with which parameters)</param>
		///// <returns></returns>
		//public static RouteData ShouldMapTo<TController>(this string relativeUrl, Expression<Func<TController, ActionResult>> action) where TController : Controller
		//{
		//	return relativeUrl.Route().ShouldMapTo(action);
		//}

		/// <summary>
		/// Verifies the <see cref="RouteData">routeData</see> maps to the controller type specified.
		/// </summary>
		/// <typeparam name="TController"></typeparam>
		/// <param name="routeData"></param>
		/// <returns></returns>
		public static RouteData ShouldMapTo<TController>(this RouteData routeData) where TController : Controller
		{
			//strip out the word 'Controller' from the type
			string expected = typeof(TController).Name.Replace("Controller", "");

			//get the key (case insensitive)
			string actual = routeData.Values.GetValue("controller").ToString();


			expected.AssertSameStringAs(actual);
			return routeData;
		}

		///// <summary>
		///// Verifies the <see cref="RouteData">routeData</see> will instruct the routing engine to ignore the route.
		///// </summary>
		///// <param name="relativeUrl"></param>
		///// <returns></returns>
		//public static RouteData ShouldBeIgnored(this string relativeUrl)
		//{
		//	RouteData routeData = relativeUrl.Route();

		//	routeData.RouteHandler.ShouldBeType<StopRoutingHandler>();
		//	return routeData;
		//}

		/// <summary>
		/// Gets a value from the <see cref="RouteValueDictionary" /> by key.  Does a
		/// case-insensitive search on the keys.
		/// </summary>
		/// <param name="routeValues"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static object GetValue(this RouteValueDictionary routeValues, string key)
		{
			foreach (var routeValueKey in routeValues.Keys)
			{
				if (string.Equals(routeValueKey, key, StringComparison.InvariantCultureIgnoreCase))
					return routeValues[routeValueKey] as string;
			}

			return null;
		}
	}
}
