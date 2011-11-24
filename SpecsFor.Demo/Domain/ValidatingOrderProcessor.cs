namespace SpecsFor.Demo.Domain
{
	public class ValidatingOrderProcessor : OrderProcessor
	{
		private readonly IValidator<Order> _validator;

		public ValidatingOrderProcessor(IValidator<Order> validator, IInventory inventory, IPublisher publisher) 
			: base(inventory, publisher)
		{
			_validator = validator;
		}

		public override OrderResult Process(Order order)
		{
			if (_validator.Validate(order))
			{
				return base.Process(order);
			}
			else
			{
				return new OrderResult {WasAccepted = false};
			}
		}
	}
}