using System;
using System.Linq.Expressions;
using OpenQA.Selenium;
using System.Linq;

namespace SpecsFor.Mvc
{
	public class FluentForm<T>
	{
		private readonly MvcWebApp _webApp;

		public FluentForm(MvcWebApp webApp)
		{
			_webApp = webApp;
		}

		public FluentField<T,TProp> Field<TProp>(Expression<Func<T, TProp>> property)
		{
			return new FluentField<T,TProp>(this, _webApp, property);
		}

		public void Submit()
		{
			//TODO: Probably not the best way to find the target form.  I'm open to suggestions.
			var forms = _webApp.Browser.FindElements(By.TagName("form"));

			if (forms.Count() == 0)
			{
				throw new ElementNotFoundException("form", "No form was found on the page.");
			}
			else if (forms.Count() > 1)
			{
				throw new MultipleMatchesException("form", "More than one form was found on the page, so the form can't be automagically submitted.  Try finding the ");
			}
			else
			{				
				forms.Single().Submit();
			}

			_webApp.Pause();
		}
	}
}