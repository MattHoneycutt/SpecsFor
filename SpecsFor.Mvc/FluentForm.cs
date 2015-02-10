using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OpenQA.Selenium;
using SpecsFor.Mvc.SeleniumExtensions;

namespace SpecsFor.Mvc
{
	public class FluentForm<TModel> where TModel : class
	{
		private IWebElement _lastField;

		public MvcWebApp WebApp { get; private set; }

		public FluentForm(MvcWebApp webApp)
		{
			WebApp = webApp;
		}

        /// <summary>
        /// Returns the field described the specified property.
        /// </summary>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns>The field.</returns>
		public FluentField<TModel,TProp> Field<TProp>(Expression<Func<TModel, TProp>> property)
		{
			var field = new FluentField<TModel, TProp>(this, WebApp, property);
			
			//Store the last field that is accessed.  This is used to submit the form.
			_lastField = field.Field;
			
			return field;
		}

        /// <summary>
        /// Selects the correct field based on the supplied value from the list of fields described by the specified property.
        /// </summary>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <returns>The field.</returns>
        public FluentField<TModel, TProp> Field<TProp>(Expression<Func<TModel, TProp>> property, string value)
        {
            var element = WebApp.FindElementsByExpressionUsingEditorConvention(property).Single(e => e.Value() == value);
            var field = new FluentField<TModel, TProp>(this, WebApp, property, element);
            _lastField = field.Field;

            return field;

        }

        /// <summary>
        /// Selects the correct field based on the supplied value from the list of fields described by the specified property.
        /// </summary>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <returns>The field.</returns>
        public FluentField<TModel, TProp> Field<TProp>(Expression<Func<TModel, TProp>> property, TProp value)
        {
	        var valueAsString = value != null ? value.ToString() : null;

            var element = WebApp.FindElementsByExpressionUsingEditorConvention(property).Single(e => e.Value().Equals(valueAsString, StringComparison.InvariantCultureIgnoreCase));
            var field = new FluentField<TModel, TProp>(this, WebApp, property, element);
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