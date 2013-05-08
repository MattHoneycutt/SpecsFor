using System;
using System.Linq.Expressions;
using OpenQA.Selenium;
using System.Linq;

namespace SpecsFor.Mvc
{
	public class FluentForm<T>
	{
		private IWebElement _lastField;

		public MvcWebApp WebApp { get; private set; }

		public FluentForm(MvcWebApp webApp)
		{
			WebApp = webApp;
		}

		public FluentField<T,TProp> Field<TProp>(Expression<Func<T, TProp>> property)
		{
			var field = new FluentField<T, TProp>(this, WebApp, property);
			
			//Store the last field that is accessed.  This is used to submit the form.
			_lastField = field.Field;
			
			return field;
		}

		public void Submit()
		{
			var submitElement = _lastField ?? FindSingleForm();

			submitElement.Submit();

			WebApp.Pause();
		}

		private IWebElement FindSingleForm()
		{
			var forms = WebApp.Browser.FindElements(By.TagName("form"));

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