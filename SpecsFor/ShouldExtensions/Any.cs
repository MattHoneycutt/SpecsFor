namespace SpecsFor.ShouldExtensions
{
	public static class Any
	{
		public static T ValueOf<T>()
		{
			Matcher.Create<T>();

			return default(T);
		}

		public static T NonDefaultValueOf<T>() where T : struct
		{
			Matcher.Create<T>(x => !Equals(x, default(T)), "Any non-default value.");

			return default(T);
		}

		public static T NonNullValueOf<T>() where T : class 
		{
			Matcher.Create<T>(x => x != null, "Any non-null value.");

			return default(T);
		}
	}
}