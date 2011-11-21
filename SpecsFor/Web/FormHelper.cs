using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using OpenQA.Selenium;

namespace SpecsFor.Web
{
	public class FormHelper<T>
	{
		private readonly MvcWebApp _webApp;

		public FormHelper(MvcWebApp webApp)
		{
			_webApp = webApp;
		}

		public FormHelper<T> SetFieldValue(Expression<Func<T, object>> property, string value)
		{
			var name = ExpressionHelper.GetExpressionText(property);

			var field = _webApp.Browser.FindElement(By.Name(name));

			field.SendKeys(value);

			return this;
		}

		public void Submit()
		{
			//TODO: Probably not the best way to find the target form.  If we have multiple
			//		forms on a page, this will break down.  I'm open to suggestions.
			_webApp.Browser.FindElement(By.TagName("form")).Submit();
		}
	}
}