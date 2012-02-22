using System;
using System.Linq;
using System.Linq.Expressions;
using OpenQA.Selenium;

namespace SpecsFor.Mvc
{
	public class FluentField<T,TProp>
	{
		private readonly FluentForm<T> _fluentForm;
		private readonly MvcWebApp _webApp;
		private readonly IWebElement _field;

		public FluentField(FluentForm<T> fluentForm, MvcWebApp webApp, Expression<Func<T, TProp>> property)
		{
			_fluentForm = fluentForm;
			_webApp = webApp;
			_field = webApp.FindElementByExpression(property);
		}

		public FluentForm<T> ShouldBeInvalid()
		{
			var validation = _webApp.Browser.FindElements(By.CssSelector("span.field-validation-error span[for=\"" + _field.GetAttribute("Name") + "\"]")).SingleOrDefault();

			if (validation == null)
			{
				throw new AssertionException("No validation message found.");
			}

			return _fluentForm;
		}

		public FluentForm<T> SetValueTo(string value)
		{
			_field.Clear();
			_field.SendKeys(value);

			return _fluentForm;
		}
	}
}