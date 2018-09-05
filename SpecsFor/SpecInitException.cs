using System;

namespace SpecsFor.Core
{
	public class SpecInitException : SpecificationException
	{
		public SpecInitException(Exception[] exceptions) : base("Init", exceptions)
		{
		}
	}
}