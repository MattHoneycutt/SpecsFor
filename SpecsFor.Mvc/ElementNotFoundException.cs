using System;

namespace SpecsFor.Mvc
{
	public class ElementNotFoundException : ApplicationException
	{
		public ElementNotFoundException(string selector, string message)
			: base(message + "  Selector used: " + selector)
		{
		}
	}
}