using SpecsFor.Mvc.Smtp.Smtp;
using SpecsFor.Mvc.Smtp.SmtpServer;

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
			MvcWebApp.AddPreTestCallback(PrepareForTest);
			_server = new Server(_portNumber);
			_server.Start();
			Processor.MessageReceived += ProcessorOnMessageReceived;
		}

		private void ProcessorOnMessageReceived(object sender, MessageEventArgs e)
		{
			Mailbox.Current.MailMessages.Add(e.Entry);
		}

		private void PrepareForTest()
		{
			Mailbox.Current = new Mailbox();
		}

		private void StopSmtp4Dev()
		{
			_server.Stop();
		}
	}
}