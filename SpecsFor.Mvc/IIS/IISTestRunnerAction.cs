using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace SpecsFor.Mvc.IIS
{
	public class IISTestRunnerAction : ITestRunnerAction
	{
		private IISExpressProcess _iisExpressProcess;
		private string _publishDir;

		public string ProjectPath { get; set; }

		public string Configuration { get; set; }

		private void StartIISExpress()
		{
			_iisExpressProcess = new IISExpressProcess(_publishDir);
			_iisExpressProcess.Start();

			MvcWebApp.BaseUrl = "http://localhost:" + _iisExpressProcess.PortNumber;
		}

		private void PublishSite(Dictionary<string, string> properties)
		{
			var logFile = Path.GetTempFileName();

			try
			{
				bool success;
				using (var projects = new ProjectCollection(properties))
				{
					projects.RegisterLogger(new FileLogger { Parameters = @"logfile=" + logFile, Verbosity = LoggerVerbosity.Quiet });
					projects.OnlyLogCriticalEvents = true;
					var project = projects.LoadProject(ProjectPath);
					success = project.Build();
				}

				if (!success)
				{
					Console.WriteLine(File.ReadAllText(logFile));
					throw new ApplicationException("Build failed.");
				}
			}
			finally
			{
				if (File.Exists(logFile))
				{
					File.Delete(logFile);
				}
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
									{"_PackageTempDir", _publishDir},
									{"AutoParameterizationWebConfigConnectionStrings", "false"}
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