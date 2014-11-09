using System;
using System.Linq.Expressions;
using OpenQA.Selenium;

namespace SpecsFor.Mvc
{
	public interface IElementConventions
	{
		By FindDisplayElementByExpressionFor<TModel, TProp>(Expression<Func<TModel, TProp>> property) where TModel : class;
		By FindEditorElementByExpressionFor<TModel, TProp>(Expression<Func<TModel, TProp>> property) where TModel : class;

		By FindValidationSummary();

		bool IsFieldInvalid(IWebElement field);
	}
}