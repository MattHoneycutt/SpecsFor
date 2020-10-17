using ExpectedObjects;

namespace SpecsFor.Shouldly
{
	public static class ExpectedObjectExtensions
	{
		public static void ShouldLookLike<T>(this T actual, T expected)
		{
			expected.ToExpectedObject().ShouldEqual(actual);
		}

		public static void ShouldLookLikePartial<T>(this T actual, object expected)
		{
			expected.ToExpectedObject().ShouldMatch(actual);
		}
	}
}