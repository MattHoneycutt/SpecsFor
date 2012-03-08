using System;
using System.Collections.Generic;
using SpecsFor.Mvc.Smtp.Mime;
using SpecsFor.Mvc.Smtp.Smtp;

namespace SpecsFor.Mvc.Smtp
{
	public class SmtpIntercepterAction : BasicTestRunnerAction
	{
		private readonly int _portNumber;
		private Server _server;
		private MvcWebApp _sut;

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
			_sut.MailMessages.Add(e.Entry);
		}

		private void PrepareForTest(MvcWebApp sut)
		{
			sut.MailMessages = new List<MailMessageEx>();
			_sut = sut;
		}

		private void StopSmtp4Dev()
		{
			_server.Stop();
		}
	}
}