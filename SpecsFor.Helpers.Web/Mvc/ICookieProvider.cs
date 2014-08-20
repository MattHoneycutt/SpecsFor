using System.Web;

namespace SpecsFor.Helpers.Web.Mvc
{
	public interface ICookieProvider
	{
		HttpCookieCollection Cookies { get; }
	}

	public class EmptyCookieProvider : ICookieProvider
	{
		public HttpCookieCollection Cookies
		{
			get
			{
				return new HttpCookieCollection();
			}
		}
	}
}