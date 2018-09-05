using System;

namespace SpecsFor.Core
{
	public class GivenSpecificationException : SpecificationException
	{
		public GivenSpecificationException(Exception[] exceptions) : base("Given", exceptions)
		{
		}
	}
}