using System;
using ExpectedObjects;
using Moq;

namespace SpecsFor.ShouldExtensions
{
	public static class Looks
	{
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