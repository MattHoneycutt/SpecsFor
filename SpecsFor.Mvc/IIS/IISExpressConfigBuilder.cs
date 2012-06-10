using System.IO;
using System.Linq;

namespace SpecsFor.Mvc.IIS
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
			var projectDirectory = new DirectoryInfo(pathToProject);
			var projectFile = projectDirectory.EnumerateFiles("*.csproj").SingleOrDefault() ??
			                  projectDirectory.EnumerateFiles("*.vbproj").SingleOrDefault();

			if (projectFile == null)
			{
				throw new FileNotFoundException("No C# or VB.NET projects were found in " + projectDirectory.FullName);
			}

			_action.ProjectPath = projectFile.FullName;
			return this;
		}

		public IISExpressConfigBuilder ApplyWebConfigTransformForConfig(string configuration)
		{
			_action.Configuration = configuration;
			return this;
		}
	}
}