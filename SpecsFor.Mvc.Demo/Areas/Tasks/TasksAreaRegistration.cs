using System.Web.Mvc;

namespace SpecsFor.Mvc.Demo.Areas.Tasks
{
	public class TasksAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "Tasks";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"Tasks_default",
				"Tasks/{controller}/{action}/{id}",
				new { controller = "List", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
