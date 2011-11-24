using Moq;
using NUnit.Framework;
using Should;
using SpecsFor.Demo.Domain;

namespace SpecsFor.Demo.OldSchoolTests
{
	public class OrderProcessorSpecs : SpecsFor<OrderProcessor>
	{
		[Test]
		public void Order_submitted_successfully_Tests()
		{
			GetMockFor<IInventory>()
				.Setup(i => i.IsQuantityAvailable("TestPart", 10))
				.Returns(true)
				.Verifiable();

			var result = SUT.Process(new Order {PartNumber = "TestPart", Quantity = 10});

			result.WasAccepted.ShouldBeTrue();

			GetMockFor<IInventory>().Verify();

			GetMockFor<IPublisher>()
				.Verify(p => p.Publish(It.Is<OrderSubmitted>(o => o.OrderNumber == result.OrderNumber)));
		}

		[Test]
		public void Order_is_rejected_Tests()
		{
			GetMockFor<IInventory>()
				.Setup(i => i.IsQuantityAvailable("TestPart", 10))
				.Returns(false)
				.Verifiable();

			var result = SUT.Process(new Order {PartNumber = "TestPart", Quantity = 10});

			result.WasAccepted.ShouldBeFalse();

			GetMockFor<IInventory>().Verify();

			GetMockFor<IPublisher>()
				.Verify(p => p.Publish(It.IsAny<OrderSubmitted>()), Times.Never());
		}
	}
}