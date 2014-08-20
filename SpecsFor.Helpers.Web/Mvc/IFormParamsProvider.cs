using System.Collections.Specialized;

namespace SpecsFor.Helpers.Web.Mvc
{
	public interface IFormParamsProvider
	{
		NameValueCollection Values { get; }
	}

	public class EmptyFormsParamProvider : IFormParamsProvider
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