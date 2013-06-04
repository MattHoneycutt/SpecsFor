using System;
using System.Diagnostics;
using System.IO;

namespace SpecsFor.Mvc.IIS
{
	internal class IISExpressProcess
	{
		private readonly string _pathToSite;
		private Process _iisProcess;

		public int PortNumber { get; private set; }

		public IISExpressProcess(string pathToSite)
		{
			_pathToSite = pathToSite;
		}

		public void Start()
		{
			PortNumber = (new Random()).Next(20000, 50000);

			var startInfo = new ProcessStartInfo
			                	{
			                		ErrorDialog = false,
			                		CreateNoWindow = true,
			                		UseShellExecute = false,
			                		Arguments = string.Format("/path:\"{0}\" /port:{1}", _pathToSite, PortNumber)
			                	};

			var programfiles = !string.IsNullOrEmpty(startInfo.EnvironmentVariables["programfiles(x86)"])
			                   	? startInfo.EnvironmentVariables["programfiles(x86)"]
			                   	: startInfo.EnvironmentVariables["programfiles"];

			var iisExpress = programfiles + "\\IIS Express\\iisexpress.exe";

			if (!File.Exists(iisExpress))
			{
				throw new FileNotFoundException(string.Format("Did not find iisexpress.exe at {0}. Ensure that IIS Express is installed to the default location.", iisExpress));
			}

			startInfo.FileName = iisExpress;

			_iisProcess = new Process { StartInfo = startInfo };
			_iisProcess.Start();
		}

		public void Stop()
		{
			if (_iisProcess != null && !_iisProcess.HasExited)
			{
				_iisProcess.CloseMainWindow();
				_iisProcess.Kill();
				_iisProcess.Dispose();
				_iisProcess = null;
			}
		}
	}
}