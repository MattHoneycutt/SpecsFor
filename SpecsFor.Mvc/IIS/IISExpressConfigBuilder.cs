using System;
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

		public IISExpressConfigBuilder With(string pathToProject, string pathToSolution = null)
		{
			var projectDirectory = new DirectoryInfo(pathToProject);
			var projectFile = projectDirectory.EnumerateFiles("*.csproj").SingleOrDefault() ??
							  projectDirectory.EnumerateFiles("*.vbproj").SingleOrDefault();

			if (projectFile == null)
			{
				throw new FileNotFoundException("No C# or VB.NET projects were found in " + projectDirectory.FullName);
			}

			_action.ProjectPath = projectFile.FullName;


			_action.SolutionPath = pathToSolution ?? FindSolution(projectDirectory);

			return this;
		}

		private static string FindSolution(DirectoryInfo projectDirectory)
		{
			if (projectDirectory.Parent == null || !projectDirectory.Parent.EnumerateFiles("*.sln").Any())
				throw new InvalidOperationException(
					"Unable to find the project's solution file!  Call 'With()' and specify the path to the solution directly.");

			if (projectDirectory.Parent.EnumerateFiles("*.sln").Count() > 1)
				throw new InvalidOperationException(
					"Multiple solution files!  Call 'With()' and specify the path to the solution directly.");

			var solutionPath = projectDirectory.Parent.EnumerateFiles("*.sln").Single().FullName;
			return solutionPath;
		}

		public IISExpressConfigBuilder Platform(string platform)
		{
			_action.Platform = platform;
			return this;
		}

		public IISExpressConfigBuilder ApplyWebConfigTransformForConfig(string configuration)
		{
			_action.Configuration = configuration;
			return this;
		}

		public IISExpressConfigBuilder CleanupPublishedFiles()
		{
			_action.CleanupPublishedFiles = true;
			return this;
		}

		/// <summary>
		/// Sets the path to an IIS application host configuration file.
		/// </summary>
		/// <param name="applicationHostConfigurationFile">The full path to the application host configuration file.</param>
		/// <returns></returns>
		/// <remarks>If a full path is not given it is assumed that the configuration file is part of the test project
		/// and will be found in the test assembly's output folder.</remarks>
		public IISExpressConfigBuilder ApplicationHostConfigurationFile(string applicationHostConfigurationFile)
		{
			var hostConfigPath = Path.GetFullPath(applicationHostConfigurationFile);

			if (File.Exists(hostConfigPath))
			{
				_action.ApplicationHostConfigurationFile = applicationHostConfigurationFile;
			}
			else
			{
				throw new FileNotFoundException("No application host configuration files were found in " + hostConfigPath);
			}

			return this;
		}

		public IISExpressConfigBuilder UseMSBuildExecutableAt(string pathToExecutable)
		{
			if (!File.Exists(pathToExecutable)) throw new FileNotFoundException(@"MSBuild.exe was not found!  Specify the full path, including \msbuild.exe!");

			_action.MSBuildOverride = pathToExecutable;

			return this;
		}

		public IISExpressConfigBuilder UsePort(int portNumber)
		{
			_action.PortNumber = portNumber;

			return this;
		}

        public IISExpressConfigBuilder UseHttps(bool useHttps)
        {
            _action.UseHttps = useHttps;

            return this;
        }

		public IISExpressConfigBuilder WithTemporaryDirectoryName(string name)
		{
			_action.TemporaryDirectoryName = name;
			return this;
		}

	    public IISExpressConfigBuilder WithOutputPath(string path)
	    {
	        _action.OutputPath = path;
	        return this;
	    }
	}
}