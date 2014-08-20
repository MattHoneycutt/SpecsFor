using System.Collections.Specialized;

namespace SpecsFor.Helpers.Web.Mvc
{
	public interface IQueryStringParamsProvider
	{
		NameValueCollection Values { get; }
	}

	public class EmptyQueryStringParamProvider : IQueryStringParamsProvider
	{
		public NameValueCollection Values
		{
			get
			{
				return new NameValueCollection();
			}
		}
	}
}