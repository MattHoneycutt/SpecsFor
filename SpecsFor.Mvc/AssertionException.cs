using System;

namespace SpecsFor.Mvc
{
	public class AssertionException : Exception
	{
		public AssertionException(string message) : base(message)
		{
		}
	}
}