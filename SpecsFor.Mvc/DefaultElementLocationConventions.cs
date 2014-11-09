using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using OpenQA.Selenium;

namespace SpecsFor.Mvc
{
	public class DefaultElementLocationConventions : IElementLocationConventions
	{
		public virtual By FindDisplayElementByExpressionFor<TModel, TProp>(Expression<Func<TModel, TProp>> property) where TModel : class
		{
			var name = ExpressionHelper.GetExpressionText(property);
			var id = TagBuilder.CreateSanitizedId(name);

			return By.Id(id);
		}

		public virtual By FindEditorElementByExpressionFor<TModel, TProp>(Expression<Func<TModel, TProp>> property) where TModel : class
		{
			var name = ExpressionHelper.GetExpressionText(property);
			var id = TagBuilder.CreateSanitizedId(name);

			return By.Id(id);
		}

		public By FindValidationSummary()
		{
			return By.ClassName("validation-summary-errors");
		}
	}
}