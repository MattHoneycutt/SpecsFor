using System;
using System.Linq;
using System.Linq.Expressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SpecsFor.Mvc
{
	public class FluentField<TModel, TProp> where TModel : class
	{
		public FluentForm<TModel> FluentForm { get; private set; }
		public MvcWebApp WebApp { get; private set; }
		public IWebElement Field { get; private set; }

		public FluentField(FluentForm<TModel> fluentForm, MvcWebApp webApp, Expression<Func<TModel, TProp>> property) 
		{
			FluentForm = fluentForm;
			WebApp = webApp;
			Field = webApp.FindElementByExpressionUsingEditorConvention(property);
		}

		public FluentForm<TModel> ShouldBeInvalid()
		{
			var validation = WebApp.Browser.FindElements(By.CssSelector("span.field-validation-error[data-valmsg-for=\"" + Field.GetAttribute("Name") + "\"]")).SingleOrDefault();

			if (validation == null)
			{
				throw new AssertionException("No validation message found.");
			}

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

        public FluentForm<TModel> SelecyByText(string text)
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