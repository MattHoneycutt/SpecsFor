using System.Diagnostics;
using System.IO;

namespace SpecsFor.Mvc
{
	public class Smtp4DevIntercepterAction : BasicTestRunnerAction
	{
		private Process _process;

		public Smtp4DevIntercepterAction()
		{
			Startup = StartSmpt4Dev;
			Shutdown = StopSmtp4Dev;
		}

		private void StartSmpt4Dev()
		{
			//TODO: We don't want to use Papercut, we want to actually intercept Emails in-process 
			//		so that we can assert off their contents. 
			var path = Path.Combine(Project.Named("HandyManager.IntegrationTests"), @"SpecsFor.Mvc\Tools\Papercut\papercut.exe");

			_process = Process.Start(path);
		}

		private void StopSmtp4Dev()
		{
			_process.CloseMainWindow();
		}
	}
}