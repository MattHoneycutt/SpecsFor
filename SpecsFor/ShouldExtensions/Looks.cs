using ExpectedObjects;
using Moq;

namespace SpecsFor.ShouldExtensions
{
	public static class Looks
	{
		public static void ShouldLookLike<T>(this T actual, T expected)
		{
			expected.ToExpectedObject().ShouldEqual(actual);
		}

		public static T Like<T>(T obj)
		{
			var expected = obj.ToExpectedObject();
			return It.Is<T>(t => expected.Equals(t));
		}
	}
}