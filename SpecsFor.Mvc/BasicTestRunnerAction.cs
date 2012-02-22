using System;

namespace SpecsFor.Mvc
{
	public class BasicTestRunnerAction : ITestRunnerAction
	{
		public Action Startup { get; protected set; }
		public Action Shutdown { get; protected set; }

		internal BasicTestRunnerAction()
		{
			Startup = () => { };
			Shutdown = () => { };
		}

		public BasicTestRunnerAction(Action startup, Action shutdown)
		{
			Startup = startup;
			Shutdown = shutdown;
		}

		void ITestRunnerAction.Startup()
		{
			Startup();
		}

		void ITestRunnerAction.Shutdown()
		{
			Shutdown();
		}
	}
}