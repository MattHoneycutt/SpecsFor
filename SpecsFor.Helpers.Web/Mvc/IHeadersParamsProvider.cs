using System.Collections.Specialized;

namespace SpecsFor.Helpers.Web.Mvc
{
	public interface IHeadersParamsProvider
	{
		NameValueCollection Values { get; }
	}

	public class EmptyHeadersParamProvider : IHeadersParamsProvider
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