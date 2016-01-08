using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Build.Utilities;

namespace SpecsFor.Mvc.IIS
{
	public class IISTestRunnerAction : ITestRunnerAction
	{
		private IISExpressProcess _iisExpressProcess;
		private string _publishDir;
		private string _intermediateDir;

		public string ProjectPath { get; set; }

		public string Configuration { get; set; }

		public string Platform { get; set; }

		public bool CleanupPublishedFiles { get; set; }

		public string ApplicationHostConfigurationFile { get; set; }

		/// <summary>
		/// Gets the name of the web application project.
		/// </summary>
		/// <remarks>This is used to tie back to the site name to configure when an application configuration file is used.</remarks>
		public string ProjectName { get; private set; }

		public string MSBuildOverride { get; set; }
		public string SolutionPath { get; set; }

		public int? PortNumber { get; set; }

        public bool UseHttps { get; set; }

		public string PublishDirectory { get; set; }

	    public string IntermediateDirectory { get; set; }

	    public string OutputPath { get; set; }

	    private void StartIISExpress()
		{
			_iisExpressProcess = new IISExpressProcess(_publishDir, ApplicationHostConfigurationFile, ProjectName);
			_iisExpressProcess.PortNumber = PortNumber;
            _iisExpressProcess.UseHttps = UseHttps;
			_iisExpressProcess.Start();

            string protocol = "http";

            if (_iisExpressProcess.UseHttps)
            {
                protocol = "https";
            }

			MvcWebApp.BaseUrl = protocol + "://localhost:" + _iisExpressProcess.PortNumber;
		}

		private void PublishSite(Dictionary<string, string> properties)
		{
			var arguments = "/p:" + string.Join(";", properties.Select(kvp => kvp.Key + "=" + kvp.Value)) + " \"" + ProjectPath + "\"";

			var msBuildPath = MSBuildOverride ??
			                  ToolLocationHelper.GetPathToBuildToolsFile("msbuild.exe", ToolLocationHelper.CurrentToolsVersion);

			var msBuildProc = new Process();
			msBuildProc.StartInfo = new ProcessStartInfo
				{
					FileName = msBuildPath,
					Arguments = arguments,
					RedirectStandardError = true,
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true,
				};
			msBuildProc.Start();
	
			var stdout = msBuildProc.StandardOutput.ReadToEnd();
			var stderr = msBuildProc.StandardError.ReadToEnd();
			msBuildProc.WaitForExit();
			
			var success = msBuildProc.ExitCode == 0;

			if (!success)
			{
				Console.WriteLine("***The publish failed.  Please ensure that your project compiles manually with MSBuild.***");
				Console.WriteLine("Here's some information to help you:");
				Console.WriteLine("MSBuild Arguments: ");
				Console.WriteLine(arguments);
				Console.WriteLine("---------------------------------------");
				Console.WriteLine("MSBuild Output:");
				Console.WriteLine(stdout);
				Console.WriteLine(stderr);
				Console.WriteLine("---------------------------------------");
				throw new ApplicationException("Build failed.");
			}
		}
		
		private void SafelyRemoveDirectory(string targetDirectory)
		{
			try
			{
				Directory.Delete(targetDirectory, true);
			}
			catch (Exception)
			{
				Console.WriteLine("There was a problem deleting {0}.", targetDirectory);
			}
		}

		public void Startup()
		{
			//TODO: Make sure the config is valid!

			if (ProjectPath.Contains("\\"))
			{
				ProjectName = ProjectPath.Substring(ProjectPath.LastIndexOf("\\") + 1);
			}
			else
			{
				ProjectName = ProjectPath;
			}

			ProjectName = ProjectName.Replace(".csproj", string.Empty).Replace(".vbproj", string.Empty);

			_publishDir = Path.Combine(Directory.GetCurrentDirectory(), PublishDirectory ?? "SpecsForMvc.TestSite");
			_intermediateDir = Path.Combine(Directory.GetCurrentDirectory(), IntermediateDirectory ?? "SpecsForMvc.TempIntermediateDir");

			var properties = new Dictionary<string, string>
								{
									{"DeployOnBuild", "true"},
									{"DeployTarget", "Package"},
									{"_PackageTempDir", "\"" + _publishDir + "\""},
									//If you think this looks bad, that's because it does.  What this
									//actually outputs looks like: "path\to\whatever\\"
									//The backslash on the end has to be escaped, otherwise msbuild.exe
									//will interpret it as escaping the final quote, which is incorrect.
									{"BaseIntermediateOutputPath", "\"" + _intermediateDir + "\\\\\""},
									{"AutoParameterizationWebConfigConnectionStrings", "false"},
									{"Platform", Platform ?? "AnyCPU" },
									//Needed for Post-Build events that reference the SolutionDir macro/property.  
									{"SolutionDir", @"""" + Path.GetDirectoryName(SolutionPath) + "\\\\\""}
								};

			if (!string.IsNullOrEmpty(Configuration))
			{
				properties.Add("Configuration", Configuration);
			}

			if (!string.IsNullOrEmpty(OutputPath))
			{
                properties.Add("OutputPath", "\"" + OutputPath + "\\\\\"");
			}

			PublishSite(properties);

			StartIISExpress();
		}

		public void Shutdown()
		{
			if (_iisExpressProcess != null)
			{
				_iisExpressProcess.Stop();

				//To make the publish faster, the published app isn't deleted.  This reduces
				//the amount of work MSBuild needs to do each time it's invoked.  It
				//can be overriden though.
				if (CleanupPublishedFiles && Directory.Exists(_publishDir))
				{
					SafelyRemoveDirectory(_publishDir);
				}

				if (CleanupPublishedFiles && Directory.Exists(_intermediateDir))
				{
					SafelyRemoveDirectory(_intermediateDir);
				}
			}
		}

    }
}