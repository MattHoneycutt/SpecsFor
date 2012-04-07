using Moq;
using NUnit.Framework;
using Should;
using SpecsFor.Demo.Domain;

namespace SpecsFor.Demo.BDD.Inheritance
{
	public class OrderProcessorSpecs
	{
		public class when_processing_an_order : given.the_item_is_available
		{
			private OrderResult _result;

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

		
		public class when_processing_an_order_with_a_negative_quantity : given.the_item_is_available
		{
			private OrderResult _result;

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

		public class given
		{
			public abstract class the_item_is_available : SpecsFor<OrderProcessor>
			{
				protected override void Given()
				{
					GetMockFor<IInventory>()
										.Setup(i => i.IsQuantityAvailable("TestPart", 10))
										.Returns(true)
										.Verifiable();
				}
			}
		}
	}
}