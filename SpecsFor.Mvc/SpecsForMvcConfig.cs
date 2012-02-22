using System;
using System.Collections.Generic;
using System.Web.Routing;

namespace SpecsFor.Mvc
{
	public class SpecsForMvcConfig
	{
		public List<ITestRunnerAction> TestRunnerActions { get; private set; }

		public SpecsForMvcConfig()
		{
			TestRunnerActions = new List<ITestRunnerAction>();
		}

		private void AddNewAction(Action action)
		{
			TestRunnerActions.Add(new BasicTestRunnerAction(action, () => { }));
		}

		public void UseBrowser(BrowserDriver driver)
		{
			AddNewAction(() => { MvcWebApp.Driver = driver; });
		}

		public void BuildRoutesUsing(Action<RouteCollection> configAction)
		{
			AddNewAction(() => configAction(RouteTable.Routes));
		}

		public void Use<TConfig>() where TConfig : SpecsForMvcConfig, new()
		{
			TestRunnerActions.AddRange(new TConfig().TestRunnerActions);
		}

		public void BeforeEachTest(Action action)
		{
			AddNewAction(() => MvcWebApp.AddPreTestCallback(action));
		}

		public void InterceptEmailMessages()
		{
			TestRunnerActions.Add(new Smtp4DevIntercepterAction());
		}

		public void AuthenticateBeforeEachTestUsing<TAuth>() where TAuth : IHandleAuthentication, new()
		{
			MvcWebApp.Authentication = new TAuth();
		}

		public IISExpressConfigBuilder UseIISExpress()
		{
			var builder = new IISExpressConfigBuilder();

			TestRunnerActions.Add(builder.GetAction());

			return builder;
		}
	}
}