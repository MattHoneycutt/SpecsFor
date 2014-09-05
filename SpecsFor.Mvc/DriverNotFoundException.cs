using System;
using OpenQA.Selenium;

namespace SpecsFor.Mvc
{
	public class DriverNotFoundException : Exception
	{
		public DriverNotFoundException(string message, DriverServiceNotFoundException innerException)
			:base (message, innerException)
		{
			
		}
	}
}