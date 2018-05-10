using System;

namespace SpecsFor
{
	public class WhenSpecificationException : SpecificationException
	{
		public WhenSpecificationException(Exception[] exceptions) : base("When", exceptions)
		{
		}
	}
}