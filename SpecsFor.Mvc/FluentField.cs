using System;
using System.Linq;
using System.Linq.Expressions;
using OpenQA.Selenium;

namespace SpecsFor.Mvc
{
	public class FluentField<T,TProp>
	{
		public FluentForm<T> FluentForm { get; private set; }
		public MvcWebApp WebApp { get; private set; }
		public IWebElement Field { get; private set; }

		public FluentField(FluentForm<T> fluentForm, MvcWebApp webApp, Expression<Func<T, TProp>> property)
		{
			FluentForm = fluentForm;
			WebApp = webApp;
			Field = webApp.FindElementByExpression(property);
		}

		public FluentForm<T> ShouldBeInvalid()
		{
			var validation = WebApp.Browser.FindElements(By.CssSelector("span.field-validation-error span[htmlfor=\"" + Field.GetAttribute("Name") + "\"]")).SingleOrDefault();

			if (validation == null)
			{
				throw new AssertionException("No validation message found.");
			}

			return FluentForm;
		}

		public FluentForm<T> SetValueTo(string value)
		{
			Field.Clear();
			Field.SendKeys(value);

			return FluentForm;
		}
	}
}