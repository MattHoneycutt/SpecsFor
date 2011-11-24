using System;
using Moq;
using NUnit.Framework;
using Should;
using SpecsFor.Demo.Domain;
using SpecsFor.ShouldExtensions;

namespace SpecsFor.Demo.ShouldExtensions
{
	public class InvoiceProcessorSpecs
	{
		public class when_processing_an_invoice_without_helpers : SpecsFor<InvoiceProcessor>
		{
			private InvoiceSubmissionResult _result;
			private Invoice _invoice;

			protected override void When()
			{
				_invoice = new Invoice
				           	{
				           		BillingName = "John D. Doe",
				           		BillingAddress = "123 Happy Street",
				           		BillingCity = "Imagination Land",
				           		BillingZip = "12345",
				           		Amount = 5.35m,
				           		Description = "Work"
				           	};
				_result = SUT.SubmitInvoice(_invoice);
			}

			[Test]
			public void then_it_returns_a_successful_result()
			{
				_result.Accepted.ShouldBeTrue();
				_result.AcceptedDate.ShouldEqual(DateTime.Today);
			}

			[Test]
			public void then_it_publishes_an_event()
			{
				GetMockFor<IPublisher>()
					.Verify(p => p.Publish(It.Is<InvoiceSubmittedEvent>(e =>
					                                                    e.BillingName == _invoice.BillingName &&
					                                                    e.BillingAddress == _invoice.BillingAddress &&
					                                                    e.BillingCity == _invoice.BillingCity &&
					                                                    e.BillingZip == _invoice.BillingZip &&
					                                                    e.Amount == _invoice.Amount &&
					                                                    e.Description == _invoice.Description)));
			}
		}

		public class when_processing_an_invoice_with_helpers : SpecsFor<InvoiceProcessor>
		{
			private InvoiceSubmissionResult _result;
			private Invoice _invoice;

			protected override void When()
			{
				_invoice = new Invoice
				           	{
				           		BillingName = "John D. Doe",
				           		BillingAddress = "123 Happy Street",
				           		BillingCity = "Imagination Land",
				           		BillingZip = "12345",
				           		Amount = 5.35m,
				           		Description = "Work"
				           	};
				_result = SUT.SubmitInvoice(_invoice);
			}

			[Test]
			public void then_it_returns_a_successful_result()
			{
				_result.ShouldLookLike(new InvoiceSubmissionResult {Accepted = true, AcceptedDate = DateTime.Today});
			}

			[Test]
			public void then_it_publishes_an_event()
			{
				GetMockFor<IPublisher>()
					.Verify(p => p.Publish(Looks.Like(new InvoiceSubmittedEvent
					                                  	{
					                                  		BillingName = _invoice.BillingName,
					                                  		BillingAddress = _invoice.BillingAddress,
					                                  		BillingCity = _invoice.BillingCity,
					                                  		BillingZip = _invoice.BillingZip,
					                                  		Amount = _invoice.Amount,
					                                  		Description = _invoice.Description
					                                  	})));
			}
		}
	}
}