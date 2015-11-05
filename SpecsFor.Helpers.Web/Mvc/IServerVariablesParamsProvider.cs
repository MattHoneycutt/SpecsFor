using System.Collections.Specialized;

namespace SpecsFor.Helpers.Web.Mvc
{
	public interface IServerVariablesParamsProvider
	{
		NameValueCollection Values { get; }
	}

	public class EmptyServerVariablessParamProvider : IServerVariablesParamsProvider
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