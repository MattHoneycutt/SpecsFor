using System;

namespace SpecsFor
{
	public class SpecInitException : SpecificationException
	{
		public SpecInitException(Exception[] exceptions) : base("Init", exceptions)
		{
		}
	}
}