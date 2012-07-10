using System.Threading;
using OpenQA.Selenium;

namespace SpecsFor.Mvc.SeleniumExtensions
{
	public static class WebElementExtensions
	{
		public static bool HasAttribute(this IWebElement element, string attribute, string value)
		{
			return element.GetAttribute(attribute) == value;
		}

		public static IWebElement ClickButton(this IWebElement element)
		{
			element.SendKeys(Keys.Enter);
			Thread.Sleep(500);

			return element;
		}

		public static string Value(this IWebElement element)
		{
			return element.GetAttribute("value");
		}
	}
}