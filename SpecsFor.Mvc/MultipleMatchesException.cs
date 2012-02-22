using System;

namespace SpecsFor.Mvc
{
	public class MultipleMatchesException : ApplicationException
	{
		public MultipleMatchesException(string selector, string message)
			: base(message + "  Selector used: " + selector)
		{
		}
	}
}