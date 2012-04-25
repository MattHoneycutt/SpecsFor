using SpecsFor.Mvc.Authentication;
using SpecsFor.Mvc.Demo.Controllers;
using SpecsFor.Mvc.Demo.Models;

namespace SpecsFor.Mvc.Demo.AcceptanceTests
{
	public class StandardAuthenticator : IHandleAuthentication
	{
		public void Authenticate(MvcWebApp mvcWebApp)
		{
			mvcWebApp.NavigateTo<AccountController>(c => c.LogOn());

			mvcWebApp.FindFormFor<LogOnModel>()
				.Field(m => m.UserName).SetValueTo("real@user.com")
				.Field(m => m.Password).SetValueTo("RealPassword")
				.Submit();
		}
	}
}