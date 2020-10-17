using System.Linq;
using NUnit.Framework;

namespace SpecsFor.Should
{
	public static class StringExtensions
	{
		public static void ShouldContainAll(this string actual, params string[] expected)
		{
			if (!expected.All(e => actual.Contains(e)))
			{
				var message = string.Format("Expected string containing all of '{0}'. \r\n Actual was '{1}'.",
											string.Join(", ", expected), actual);

				Assert.Fail(message);
			}
		}
	}
}