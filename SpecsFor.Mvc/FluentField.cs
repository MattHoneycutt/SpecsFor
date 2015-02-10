using System;
using System.Linq;
using System.Linq.Expressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SpecsFor.Mvc.SeleniumExtensions;

namespace SpecsFor.Mvc
{
	public class FluentField<TModel, TProp> where TModel : class
	{
		private readonly Expression<Func<TModel, TProp>> _property;

		public FluentForm<TModel> FluentForm { get; private set; }
		public MvcWebApp WebApp { get; private set; }
		public IWebElement Field { get; private set; }

		public FluentField(FluentForm<TModel> fluentForm, MvcWebApp webApp, Expression<Func<TModel, TProp>> property) 
		{
			_property = property;
			FluentForm = fluentForm;
			WebApp = webApp;
			Field = webApp.FindElementByExpressionUsingEditorConvention(property);
		}

        public FluentField(FluentForm<TModel> fluentForm, MvcWebApp webApp, Expression<Func<TModel, TProp>> property, IWebElement webElement)
        {
            _property = property;
            FluentForm = fluentForm;
            WebApp = webApp;
            Field = webElement;
        }
        
		public FluentForm<TModel> ShouldBeInvalid()
		{
			if (!WebApp.IsFieldInvalidByConvention(Field))
				throw new AssertionException("Field is not marked as invalid!");

			return FluentForm;
		}

		public FluentForm<TModel> ValueShouldEqual(string value)
		{
			if (!string.Equals(Field.Value(), value))
				throw new AssertionException(
					string.Format("Field for {0} does not have expected value. \r\n\tExpected: {1}\r\n\tActual: {2}", _property, value, Field.Value()));

			return FluentForm;
		}

        public FluentForm<TModel> SetValueTo(string value)
        {
            Field.Clear();
            Field.SendKeys(value);

            WebApp.Pause();

            return FluentForm;
        }
        public FluentForm<TModel> SelectByValue(string value)
        {
            //create select element object 
            var selectElement = new SelectElement(Field);
                                
            //select by value
            selectElement.SelectByValue(value);

            WebApp.Pause();

            return FluentForm;
        }

        public FluentForm<TModel> SelectByText(string text)
        {
            //create select element object 
            var selectElement = new SelectElement(Field);

            // select by text
            selectElement.SelectByText(text);

            WebApp.Pause();

            return FluentForm;
        }

		public FluentForm<TModel> Click()
		{
			Field.Click();

			return FluentForm;
		} 

		public FluentForm<TModel> InteractWithField(Action<IWebElement> callback)
		{
			callback(Field);

			return FluentForm;
		}
	}
}