using System.Web.Mvc;
using SpecsForWebHelpers.Web.Domain;

namespace SpecsForWebHelpers.Web.Filters
{
	public class MattOnlyAttribute : ActionFilterAttribute
	{
		public ICurrentUser CurrentUser { get; set; }

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (CurrentUser.UserName != "Matt")
			{
				filterContext.Result =
					new ViewResult { ViewName = "YouAreNotMatt" };
			}
		}
	}
}