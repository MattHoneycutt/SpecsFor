using Moq;
using NUnit.Framework;
using Should;
using SpecsFor.Demo.Domain;

namespace SpecsFor.Demo.BDD.Composition
{
	public class ValidatingOrderProcessorSpecs
	{
		public class when_processing_an_order : SpecsFor<ValidatingOrderProcessor>
		{
			private OrderResult _result;

			protected override void Given()
			{
				Given<the_item_is_available>();
				Given<the_item_is_valid>();

				base.Given();
			}

			protected override void When()
			{
				_result = SUT.Process(new Order { PartNumber = "TestPart", Quantity = 10 });
			}

			[Test]
			public void then_the_order_is_accepted()
			{
				_result.WasAccepted.ShouldBeTrue();	
			}

			[Test]
			public void then_it_checks_the_inventory()
			{
				GetMockFor<IInventory>().Verify();
			}

			[Test]
			public void then_it_raises_an_order_submitted_event()
			{
				GetMockFor<IPublisher>()
					.Verify(p => p.Publish(It.Is<OrderSubmitted>(o => o.OrderNumber == _result.OrderNumber)));
			}
		}


		public class when_processing_an_order_with_a_negative_quantity : SpecsFor<ValidatingOrderProcessor>
		{
			private OrderResult _result;

			protected override void Given()
			{
				Given<the_item_is_available>();
				Given<the_item_is_not_valid>();

				base.Given();
			}

			protected override void When()
			{
				_result = SUT.Process(new Order{ PartNumber = "TestPart", Quantity = -1});
			}

			[Test]
			public void then_the_order_is_rejected()
			{
				_result.WasAccepted.ShouldBeFalse();
			}

			[Test]
			public void then_it_does_not_check_the_inventory()
			{
				GetMockFor<IInventory>()
					.Verify(i => i.IsQuantityAvailable("TestPart", -1), Times.Never());
			}

			[Test]
			public void then_it_does_not_raise_an_order_submitted_event()
			{
				GetMockFor<IPublisher>()
					.Verify(p => p.Publish(It.IsAny<OrderSubmitted>()), Times.Never());
			}
		}

		public class the_item_is_available : IContext<ValidatingOrderProcessor>
		{
			public void Initialize(ISpecs<ValidatingOrderProcessor> state)
			{
				state.GetMockFor<IInventory>()
					.Setup(i => i.IsQuantityAvailable("TestPart", 10))
					.Returns(true)
					.Verifiable();
			}
		}

		public class the_item_is_valid : IContext<ValidatingOrderProcessor>
		{
			public void Initialize(ISpecs<ValidatingOrderProcessor> state)
			{
				state.GetMockFor<IValidator<Order>>()
					.Setup(v => v.Validate(It.IsAny<Order>()))
					.Returns(true);
			}
		}

		public class the_item_is_not_valid : IContext<ValidatingOrderProcessor>
		{
			public void Initialize(ISpecs<ValidatingOrderProcessor> state)
			{
				state.GetMockFor<IValidator<Order>>()
					.Setup(v => v.Validate(It.IsAny<Order>()))
					.Returns(false);
			}
		}
	}
}