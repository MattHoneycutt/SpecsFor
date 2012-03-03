using System.Diagnostics;
using System.IO;
using SpecsFor.Mvc.Smtp.Smtp;

namespace SpecsFor.Mvc.Smtp
{
	public class SmtpIntercepterAction : BasicTestRunnerAction
	{
		private readonly int _portNumber;
		private Server _server;

		public SmtpIntercepterAction(int portNumber)
		{
			_portNumber = portNumber;
			Startup = StartSmpt4Dev;
			Shutdown = StopSmtp4Dev;
		}

		private void StartSmpt4Dev()
		{
			_server = new Server(_portNumber);
			_server.Start();
		}

		private void StopSmtp4Dev()
		{
			_server.Stop();
		}
	}
}