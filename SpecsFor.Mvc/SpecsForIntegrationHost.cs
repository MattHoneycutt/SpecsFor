using System.Linq;

namespace SpecsFor.Mvc
{
	public class SpecsForIntegrationHost
	{
		private readonly SpecsForMvcConfig _config;

		public SpecsForIntegrationHost(SpecsForMvcConfig config)
		{
			_config = config;
		}

		public void Start()
		{
			//Clear out existing MvcWebApp configuration that may be left-over
			//from a previous host that was created in this app.
			MvcWebApp.PreTestCallbacks.Clear();
			foreach (var action in _config.TestRunnerActions)
			{
				action.Startup();
			}
		}

		public void Shutdown()
		{
			foreach (var action in _config.TestRunnerActions.AsEnumerable().Reverse())
			{
				action.Shutdown();
			}
		}
	}
}