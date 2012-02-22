using OpenQA.Selenium;

namespace SpecsFor.Mvc.SeleniumExtensions
{
	public static class WebElementExtensions
	{
		 public static bool HasAttribute(this IWebElement element, string attribute, string value)
		 {
		 	return element.GetAttribute(attribute) == value;
		 }
	}
}