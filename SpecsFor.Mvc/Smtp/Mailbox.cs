using System.Collections.Generic;
using SpecsFor.Mvc.Smtp.Mime;

namespace SpecsFor.Mvc.Smtp
{
	public static class MailboxHelper
	{
		public static Mailbox Mailbox(this MvcWebApp app)
		{
			return Mvc.Smtp.Mailbox.Current;
		}
	}

	public class Mailbox
	{
		//I'm really not a fan of this, but it's simple and works.  
		public static Mailbox Current;

		public IList<MailMessageEx> MailMessages { get; private set; }

		public Mailbox()
		{
			MailMessages = new List<MailMessageEx>();
		}
	}
}