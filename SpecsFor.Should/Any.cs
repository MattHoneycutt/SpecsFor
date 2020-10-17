using System;

namespace SpecsFor.Should
{
	public static class Any
	{
		public static T ValueOf<T>()
		{
			Matcher.Create<T>(null, "Any value of type " + typeof(T).FullName);

			return default(T);
		}

		public static T NonDefaultValueOf<T>() where T : struct
		{
			Matcher.Create<T>(x => !Equals(x, default(T)), "Non-default value of " + typeof(T).FullName);

			return default(T);
		}

		public static T NonNullValueOf<T>()
		{
		    if (default(T) != null)
		        throw new InvalidOperationException("You cannot use this method with a non-nullable type.");

			Matcher.Create<T>(x => x != null, "Non-null value of " + typeof(T).FullName);

			return default(T);
		}
	}
}