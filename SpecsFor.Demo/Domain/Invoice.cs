namespace SpecsFor.Demo.Domain
{
	public class Invoice
	{
		public string BillingName { get; set; }

		public string BillingAddress { get; set; }

		public string BillingCity { get; set; }

		public string BillingZip { get; set; }

		public decimal Amount { get; set; }

		public string Description { get; set; }
	}
}