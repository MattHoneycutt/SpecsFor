using System.IO;
using System.Linq;

namespace SpecsFor.Mvc
{
	public class IISExpressConfigBuilder
	{
		private readonly IISTestRunnerAction _action;

		public IISExpressConfigBuilder()
		{
			_action = new IISTestRunnerAction();
		}

		internal ITestRunnerAction GetAction()
		{
			return _action;
		}

		public IISExpressConfigBuilder With(string pathToProject)
		{
			var projectFile = new DirectoryInfo(pathToProject).EnumerateFiles("*.csproj").Single().FullName;
			_action.ProjectPath = projectFile;
			return this;
		}

		public IISExpressConfigBuilder ApplyWebConfigTransformForConfig(string configuration)
		{
			_action.Configuration = configuration;
			return this;
		}
	}
}