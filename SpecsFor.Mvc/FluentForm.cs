using System;
using System.Linq.Expressions;
using OpenQA.Selenium;
using System.Linq;

namespace SpecsFor.Mvc
{
	public class FluentForm<T>
	{
		private readonly MvcWebApp _webApp;
		private IWebElement _lastField;

		public FluentForm(MvcWebApp webApp)
		{
			_webApp = webApp;
		}

		public FluentField<T,TProp> Field<TProp>(Expression<Func<T, TProp>> property)
		{
			var field = new FluentField<T, TProp>(this, _webApp, property);
			
			//Store the last field that is accessed.  This is used to submit the form.
			_lastField = field.Field;
			
			return field;
		}

		public void Submit()
		{
			var submitElement = _lastField ?? FindSingleForm();

			submitElement.Submit();

			_webApp.Pause();
		}

		private IWebElement FindSingleForm()
		{
			var forms = _webApp.Browser.FindElements(By.TagName("form"));

			if (!forms.Any())
			{
				throw new ElementNotFoundException("form", "No form was found on the page.");
			}
			else if (forms.Count() > 1)
			{
				throw new MultipleMatchesException("form", "More than one form was found on the page, so the form can't be automagically submitted.  Populate an element in the form first, or manually find and submit the form.");
			}
			else
			{
				return forms.Single();
			}
		}
	}
}