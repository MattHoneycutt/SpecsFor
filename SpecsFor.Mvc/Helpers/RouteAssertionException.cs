namespace SpecsFor.Mvc.Helpers
{
	public class RouteAssertionException : AssertionException
	{
		public RouteAssertionException(string paramName, object expectedValue, object actualValue)
			: base (string.Format("Route parameter '{0}' does not match. Expected '{1}', found '{2}'", paramName, expectedValue, actualValue))
		{
		}
	}
}