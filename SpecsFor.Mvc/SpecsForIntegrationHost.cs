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