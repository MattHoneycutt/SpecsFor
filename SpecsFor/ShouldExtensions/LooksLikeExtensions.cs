using ExpectedObjects;

namespace SpecsFor.Tests.ShouldExtensions
{
	public static class LooksLikeExtensions
	{
		public static void ShouldLookLike<T>(this T actual, T expected)
		{
			expected.ToExpectedObject().ShouldEqual(actual);
		}
	}
}