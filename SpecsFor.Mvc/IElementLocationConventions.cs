using System;
using System.Linq.Expressions;
using OpenQA.Selenium;

namespace SpecsFor.Mvc
{
	public interface IElementLocationConventions
	{
		By FindDisplayElementByExpressionFor<TModel, TProp>(Expression<Func<TModel, TProp>> property) where TModel : class;
		By FindEditorElementByExpressionFor<TModel, TProp>(Expression<Func<TModel, TProp>> property) where TModel : class;
		By FindValidationSummary();
	}
}