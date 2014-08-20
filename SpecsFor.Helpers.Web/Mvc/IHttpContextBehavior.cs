namespace SpecsFor.Helpers.Web.Mvc
{
	public interface IHttpContextBehavior
	{
		bool IsDebuggingEnabled { get; }
	}

	public class FakeHttpContextBehavior : IHttpContextBehavior
	{
		public bool IsDebuggingEnabled { get { return true; } }
	}
}