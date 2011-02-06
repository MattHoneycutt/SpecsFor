using System;
using System.Linq;

namespace SpecsFor
{
	public class InvalidContextException : ApplicationException
	{
		public InvalidContextException(Type[] badTypes)
			: base(GetMessage(badTypes))
		{
		}

		private static string GetMessage(Type[] badTypes)
		{
			return "The following context types do not implement a suitable context interface: " +
			       string.Join(", ", badTypes.Select(t => t.Name).ToArray());
		}
	}
}