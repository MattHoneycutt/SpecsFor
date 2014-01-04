using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SpecsFor.Mvc.IIS
{
	public class IISTestRunnerAction : ITestRunnerAction
	{
		private IISExpressProcess _iisExpressProcess;
		private string _publishDir;

		public string ProjectPath { get; set; }

		public string Configuration { get; set; }

		public string Platform { get; set; }

		private void StartIISExpress()
		{
			_iisExpressProcess = new IISExpressProcess(_publishDir);
			_iisExpressProcess.Start();

			MvcWebApp.BaseUrl = "http://localhost:" + _iisExpressProcess.PortNumber;
		}

		private void PublishSite(Dictionary<string, string> properties)
		{
			var arguments = "/p:" + string.Join(";", properties.Select(kvp => kvp.Key + "=" + kvp.Value)) + " \"" + ProjectPath + "\"";

			var msBuildProc = new Process();
			msBuildProc.StartInfo = new ProcessStartInfo
				{
					FileName = Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "msbuild.exe"),
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
				Console.WriteLine("The publish failed.  Dumping MSBuild output:");
				Console.WriteLine(stdout);
				Console.WriteLine(stderr);
				throw new ApplicationException("Build failed.");
			}
		}

		public void Startup()
		{
			//TODO: Make sure the config is valid!

			_publishDir = Path.Combine(Directory.GetCurrentDirectory(), "SpecsForMvc.TestSite");

			var properties = new Dictionary<string, string>
			                 	{
									{"DeployOnBuild", "true"},
									{"DeployTarget", "Package"},
									{"_PackageTempDir", "\"" + _publishDir + "\""},
									{"AutoParameterizationWebConfigConnectionStrings", "false"},
									{"Platform", Platform ?? "AnyCPU" },
									//Needed for Post-Build events that reference the SolutionDir macro/property.  
									{"SolutionDir", "\"" + Directory.GetParent(Path.GetDirectoryName(ProjectPath)).FullName + "\\\\\""}
			                 	};

			if (!string.IsNullOrEmpty(Configuration))
			{
				properties.Add("Configuration", Configuration);
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
				//the amount of work MSBuild needs to do each time it's invoked.  This
				//might need to be a configurable option in some scenarios though.
				//if (Directory.Exists(_publishDir))
				//{
				//    Directory.Delete(_publishDir, true);
				//}
			}
		}
	}
}