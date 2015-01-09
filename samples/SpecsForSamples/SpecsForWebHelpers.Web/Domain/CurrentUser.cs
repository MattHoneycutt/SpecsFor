using System.Web;

namespace SpecsForWebHelpers.Web.Domain
{
	public interface ICurrentUser
	{
		string UserName { get; }
		void SetName(string name);
	}

	public class CurrentUser : ICurrentUser
	{
		private readonly HttpSessionStateBase _session;

		public CurrentUser(HttpSessionStateBase session)
		{
			_session = session;
		}

		public string UserName
		{
			get { return (string)(_session["name"] ?? "Unknown"); }
		}

		public void SetName(string name)
		{
			_session["name"] = name;
		}
	}
}