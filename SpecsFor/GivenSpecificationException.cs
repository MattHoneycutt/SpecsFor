using System;

namespace SpecsFor
{
	public class GivenSpecificationException : SpecificationException
	{
		public GivenSpecificationException(Exception[] exceptions) : base("Given", exceptions)
		{
		}
	}
}