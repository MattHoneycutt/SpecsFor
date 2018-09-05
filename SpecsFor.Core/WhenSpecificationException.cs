using System;

namespace SpecsFor.Core
{
	public class WhenSpecificationException : SpecificationException
	{
		public WhenSpecificationException(Exception[] exceptions) : base("When", exceptions)
		{
		}
	}
}