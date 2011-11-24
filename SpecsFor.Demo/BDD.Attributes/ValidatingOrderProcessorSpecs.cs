using System;
using Moq;
using NUnit.Framework;
using Should;
using SpecsFor.Demo.Domain;

namespace SpecsFor.Demo.BDD.Attributes
{
	public class ValidatingOrderProcessorSpecs
	{
		[Given(typeof(the_item_is_available), typeof(the_order_is_valid))]
		public class when_processing_an_order : SpecsFor<ValidatingOrderProcessor>
		{
			private OrderResult _result;

			public when_processing_an_order(Type[] contexts) : base(contexts){}

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

		[Given(typeof(the_order_is_not_valid))]
		[Given(typeof(the_item_is_not_available))]
		public class when_processing_an_an_invalid_order : SpecsFor<ValidatingOrderProcessor>
		{
			private OrderResult _result;

			public when_processing_an_an_invalid_order(Type[] context) : base(context) {}

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
			public void then_it_does_not_raise_an_order_submitted_event()
			{
				GetMockFor<IPublisher>()
					.Verify(p => p.Publish(It.IsAny<OrderSubmitted>()), Times.Never());
			}
		}

		public class the_item_is_available : IContext<ValidatingOrderProcessor>
		{
			public void Initialize(ITestState<ValidatingOrderProcessor> state)
			{
				state.GetMockFor<IInventory>()
					.Setup(i => i.IsQuantityAvailable("TestPart", 10))
					.Returns(true)
					.Verifiable();
			}
		}

		public class the_order_is_valid : IContext<ValidatingOrderProcessor>
		{
			public void Initialize(ITestState<ValidatingOrderProcessor> state)
			{
				state.GetMockFor<IValidator<Order>>()
					.Setup(v => v.Validate(It.IsAny<Order>()))
					.Returns(true);
			}
		}

		public class the_order_is_not_valid : IContext<ValidatingOrderProcessor>
		{
			public void Initialize(ITestState<ValidatingOrderProcessor> state)
			{
				state.GetMockFor<IValidator<Order>>()
					.Setup(v => v.Validate(It.IsAny<Order>()))
					.Returns(false);
			}
		}

		public class the_item_is_not_available : IContext<ValidatingOrderProcessor>
		{
			public void Initialize(ITestState<ValidatingOrderProcessor> state)
			{
				state.GetMockFor<IInventory>()
					.Setup(i => i.IsQuantityAvailable(It.IsAny<string>(), It.IsAny<int>()))
					.Returns(false);
			}
		}
	}
}