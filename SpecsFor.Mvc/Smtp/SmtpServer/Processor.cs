// Borrowed from the Papercut project: papercut.codeplex.com.  
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SpecsFor.Mvc.Smtp.Mime;

namespace SpecsFor.Mvc.Smtp.SmtpServer
{
	public class Processor
	{
		#region Command Parser

		public static void ProcessCommand(Connection connection, object data)
		{
			if (data is byte[])
			{
				connection.Session.Message = (byte[])data;
				connection.Send("250 OK");
			}
			else
			{
				// Command mode
				string command = (string)data;
				string[] parts = command.Split(' ');

				switch (parts[0].ToUpper())
				{
					case "HELO":
						HELO(connection, parts);
						break;

					case "EHLO":
						EHLO(connection, parts);
						break;

					case "SEND":
					case "SOML":
					case "SAML":
					case "MAIL":
						MAIL(connection, parts);
						break;

					case "RCPT":
						RCPT(connection, parts);
						break;

					case "DATA":
						DATA(connection);
						break;

					case "VRFY":
						connection.Send("252 Cannot VRFY user, but will accept message and attempt delivery");
						break;

					case "EXPN":
						connection.Send("252 Cannot expand upon list");
						break;

					case "RSET":
						connection.Session.Reset();
						connection.Send("250 OK");
						break;

					case "NOOP":
						connection.Send("250 OK");
						break;

					case "QUIT":
						connection.Send("221 Goodbye!");
						connection.Close();
						break;

					case "HELP":
					case "TURN":
						connection.Send("502 Command not implemented");
						break;

					default:
						connection.Send("500 Command not recognized");
						break;
				}
			}
		}

		#endregion

		#region Command Handlers

		#region HELO
		private static void HELO(Connection connection, string[] parts)
		{
			connection.Session.Sender = parts.Length < 2 ? string.Empty : parts[1];
			connection.Send("250 {0}", Dns.GetHostName().ToLower());
		}

		#endregion

		#region EHLO
		private static void EHLO(Connection connection, string[] parts)
		{
			connection.Session.Sender = parts.Length < 2 ? string.Empty : parts[1];
			connection.Send("250-{0}", Dns.GetHostName().ToLower());
			connection.Send("250-8BITMIME");
			connection.Send("250 OK");
		}
		#endregion

		#region MAIL
		private static void MAIL(Connection connection, string[] parts)
		{
			string line = string.Join(" ", parts);

			// Check for the right number of parameters
			if (parts.Length < 2)
			{
				connection.Send("504 Command parameter not implemented");
				return;
			}

			// Check for the ":"
			if (!parts[1].ToUpper().StartsWith("FROM") || !line.Contains(":"))
			{
				connection.Send("504 Command parameter not implemented");
				return;
			}

			// Check command order
			if (connection.Session.Sender == null)
			{
				connection.Send("503 Bad sequence of commands");
				return;
			}

			// Set the from settings
			connection.Session.Reset();
			string address = line.Substring(line.IndexOf(":") + 1).Replace("<", string.Empty).Replace(">", string.Empty).Trim();
			connection.Session.MailFrom = address;

			// Check for encoding
			foreach (string part in parts)
			{
				if (part.ToUpper().StartsWith("BODY="))
				{
					switch (part.ToUpper().Replace("BODY=", string.Empty).Trim())
					{
						case "8BITMIME":
							connection.Session.UseUtf8 = true;
							break;
						default:
							connection.Session.UseUtf8 = false;
							break;
					}
					break;
				}
			}

			connection.Send("250 <{0}> OK", address);
		}
		#endregion

		#region RCPT
		private static void RCPT(Connection connection, string[] parts)
		{
			string line = string.Join(" ", parts);

			// Check for the ":"
			if (!line.ToUpper().StartsWith("RCPT TO") || !line.Contains(":"))
			{
				connection.Send("504 Command parameter not implemented");
				return;
			}

			// Check command order
			if (connection.Session.Sender == null || connection.Session.MailFrom == null)
			{
				connection.Send("503 Bad sequence of commands");
				return;
			}

			string address = line.Substring(line.IndexOf(":") + 1).Replace("<", string.Empty).Replace(">", string.Empty).Trim();
			if (!connection.Session.Recipients.Contains(address))
				connection.Session.Recipients.Add(address);
			connection.Send("250 <{0}> OK", address);
		}
		#endregion

		#region DATA
		private static void DATA(Connection connection)
		{
			// Check command order
			if (connection.Session.Sender == null || connection.Session.MailFrom == null || connection.Session.Recipients.Count == 0)
			{
				connection.Send("503 Bad sequence of commands");
				return;
			}

			string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("yyyyMMddHHmmssFF") + ".eml");

			try
			{
				var lines = new StringBuilder();

				using (var networkStream = new NetworkStream(connection.Client, false))
				using (var reader = new StreamReader(networkStream))
				{
					connection.Send("354 Start mail input; end with <CRLF>.<CRLF>").AsyncWaitHandle.WaitOne();
					string line;

					while ((line = reader.ReadLine()) != ".")
						lines.AppendLine(line);
				}

				var mimeReader = new MimeReader(lines.ToString().Replace("\r\n", "\n").Split('\n'));
				var entity = mimeReader.CreateMimeEntity();
				var message = entity.ToMailMessageEx();
				OnMessageReceived(connection, message);
			}
			catch(IOException e)
			{
				Logger.WriteWarning("IOException received in Processor.DATA while reading message.  Closing connection.  Message: " + e.Message, connection.ConnectionId);
				connection.Close();
				return;
			}

			connection.Send("250 OK");
		}
		#endregion

		#endregion

		private static void OnMessageReceived(Connection connection, MailMessageEx message)
		{
			if (MessageReceived != null)
				MessageReceived(connection, new MessageEventArgs(message));
		}

		public static event EventHandler<MessageEventArgs> MessageReceived;
	}

	public class MessageEventArgs : EventArgs
	{
		public MailMessageEx Entry;
		public MessageEventArgs(MailMessageEx entry)
		{
			Entry = entry;
		}
	}
}