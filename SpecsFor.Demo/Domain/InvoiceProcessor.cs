using System;

namespace SpecsFor.Demo.Domain
{
	public class InvoiceProcessor
	{
		private readonly IPublisher _publisher;

		public InvoiceProcessor(IPublisher publisher)
		{
			_publisher = publisher;
		}

		public InvoiceSubmissionResult SubmitInvoice(Invoice invoice)
		{
			_publisher.Publish(new InvoiceSubmittedEvent
			                   	{
			                   		BillingAddress = invoice.BillingAddress,
									BillingCity = invoice.BillingCity,
									BillingName = invoice.BillingName,
									BillingZip = invoice.BillingZip,
									Amount = invoice.Amount,
									Description = invoice.Description
			                   	});

			return new InvoiceSubmissionResult{Accepted = true, AcceptedDate = DateTime.Today};
		}
	}
}