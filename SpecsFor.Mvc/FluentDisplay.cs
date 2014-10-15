using System;
using System.Linq.Expressions;
using OpenQA.Selenium;

namespace SpecsFor.Mvc
{
	public class FluentDisplay<TModel> where TModel : class
	{
		public MvcWebApp WebApp { get; private set; }

		public FluentDisplay(MvcWebApp webApp)
		{
			WebApp = webApp;
		}

		//TODO: Do we wrap this in a strongly-typed wrapper to make it easier to work with?
		public IWebElement DisplayFor<TProp>(Expression<Func<TModel, TProp>> property)
		{
			return WebApp.FindElementByExpressionUsingDisplayConvention(property);
		}
	}
}