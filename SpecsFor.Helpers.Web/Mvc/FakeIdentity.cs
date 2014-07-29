using System;
using System.Security.Principal;

namespace SpecsFor.Helpers.Web.Mvc
{
	public class FakeIdentity : IIdentity
	{
		public string Name { get; set; }

		public FakeIdentity(string userName)
		{
			Name = userName;

		}

		public string AuthenticationType
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsAuthenticated
		{
			get { return !String.IsNullOrEmpty(Name); }
		}
	}
}