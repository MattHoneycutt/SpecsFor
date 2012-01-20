using System;
using ExpectedObjects;
using Moq;

namespace SpecsFor.ShouldExtensions
{
	public static class Looks
	{
		//TODO: Move to their own class in SpecsFor v3.
		public static void ShouldLookLike<T>(this T actual, T expected)
		{
			expected.ToExpectedObject().ShouldEqual(actual);
		}

		public static void ShouldLookLikePartial<T>(this T actual, object expected)
		{
			expected.ToExpectedObject().ShouldMatch(actual);
		}

		public static T Like<T>(T obj)
		{
			var expected = obj.ToExpectedObject();
			return It.Is<T>(t => expected.Equals(t));
		}

		public static T LikePartialOf<T>(object partial)
		{
			var expected = partial.ToExpectedObject();

			return It.Is<T>(t => ShouldMatch(expected, t));
		}

		private static bool ShouldMatch(ExpectedObject expected, object o)
		{
			try
			{
				expected.ShouldMatch(o);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}