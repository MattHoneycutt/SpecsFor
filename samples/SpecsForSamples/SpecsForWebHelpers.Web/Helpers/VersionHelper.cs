using System.Web.Mvc;

namespace SpecsForWebHelpers.Web.Helpers
{
	public static class VersionHelper
	{
		public static string GetVersionString(this HtmlHelper helper)
		{
			if (helper.ViewContext.HttpContext.IsDebuggingEnabled)
			{
				return "1.0.0.0-DEBUG";
			}
			else
			{
				return "1.0.0.0";
			}
		}
	}
}