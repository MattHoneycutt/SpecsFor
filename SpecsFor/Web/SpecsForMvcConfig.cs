using System;
using System.Diagnostics;
using System.IO;
using System.Web.Routing;

namespace SpecsFor.Web
{
	public abstract class SpecsForMvcConfig
	{
		private string _pathForIISExpress;
		private Process _iisProcess;

		protected void UseBrowser(BrowserDriver driver)
		{
			MvcWebApp.Driver = driver.CreateDriver;
		}

		protected void BuildRoutesUsing(Action<RouteCollection> configAction)
		{
			configAction(RouteTable.Routes);
		}

		protected string Project(string projectName)
		{
			var directory = new DirectoryInfo(Environment.CurrentDirectory);

			while (directory.GetFiles("*.sln").Length == 0)
			{
				directory = directory.Parent;
			}

			return Path.Combine(directory.FullName, projectName);
		}
		
		protected void UseIISExpressWith(string pathForIISExpress)
		{
			_pathForIISExpress = pathForIISExpress;
		}

		private void StartIISExpress()
		{
			var portNumber = (new Random()).Next(20000, 50000);

			var startInfo = new ProcessStartInfo
			                	{
			                		WindowStyle = ProcessWindowStyle.Normal,
			                		ErrorDialog = false,
			                		CreateNoWindow = false,
			                		UseShellExecute = false,
			                		Arguments = string.Format("/path:\"{0}\" /port:{1}", _pathForIISExpress, portNumber)
			                	};

			var programfiles = !string.IsNullOrEmpty(startInfo.EnvironmentVariables["programfiles(x86)"])
			                   	? startInfo.EnvironmentVariables["programfiles(x86)"]
			                   	: startInfo.EnvironmentVariables["programfiles"];

			startInfo.FileName = programfiles + "\\IIS Express\\iisexpress.exe";

			_iisProcess = new Process { StartInfo = startInfo };
			_iisProcess.Start();

			MvcWebApp.BaseUrl = "http://localhost:" + portNumber;
		}

		public virtual void SetupTestRun()
		{
			if (!string.IsNullOrEmpty(_pathForIISExpress))
			{
				StartIISExpress();
			}
		}

		public virtual void TearDownTestRun()
		{
			if (_iisProcess != null && !_iisProcess.HasExited)
			{
				_iisProcess.CloseMainWindow();
				_iisProcess.Dispose();
			}

			//TODO: Any other cleanup?
		}
	}
}