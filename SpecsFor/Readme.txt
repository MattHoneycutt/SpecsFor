------------------------------------------
SpecsFor - The Best BDD Framework For .NET
------------------------------------------
Thanks for installing SpecsFor, the Behavior-Driven Development framework that is optimized for *your* productivity.  
Questions or suggestions?  Contact me anytime on Twitter (@matthoneycutt) or at my site (http://trycatchfail.com).

---------------
Common Problems
---------------
Problem: "I'm getting an error about the wrong version of [StructureMap/NUnit/whatever]!"
Solution: Open the Visual Studio Package Manager Console (click View, Other Windows, Package Manager Console), 
		  select your test project in the "Default Project" dropdown, then type and run this command:
		  Add-BindingRedirect

Got another problem?  Hit me up on Twitter (@matthoneycutt) for a quick response! 

---------------
Getting Started
---------------
The easiest way to get started is to create your first spec!  Let's create a simple test case:

	using SpecsFor;
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
	}

SpecsFor takes care of managing your mocks and creating your system-under-test (SUT) for you automatically!  But
this test is still ugly, let's refactor it to be more BDD:

	using SpecsFor;
	public class OrderProcessorSpecs : SpecsFor<OrderProcessor>
	{
		ProcessingResult _result;

		protected override void Given()
		{
			GetMockFor<IInventory>()
				.Setup(i => i.IsQuantityAvailable("TestPart", 10))
				.Returns(true)
				.Verifiable();
		}

		protected override void When()
		{
			_result = SUT.Process(new Order {PartNumber = "TestPart", Quantity = 10});
		}

		[Test]
		public void then_the_order_is_accepted()
		{
			_result.WasAccepted.ShouldBeTrue();
		}
 
		[Test]
		public void then_it_checks_the_quantity()
		{
			GetMockFor<IInventory>().Verify();
		}
 
		[Test]
		public void then_it_raises_a_new_event()
		{
			GetMockFor<IPublisher>()
				.Verify(p => p.Publish(It.Is<OrderSubmitted>(o => o.OrderNumber == result.OrderNumber)));
		}
	}

We now have a single assert per test method (a best-practice in testing), and our test setup is cleanly
separated from the action we're actually testing.

For more information about getting started, check out the examples at http://specsfor.com. 

------------------
What's in the Box? 
------------------
Aside from the base SpecsFor<T> class (which includes an auto-mocking container), there are numerous helpers
to simplify your testing life, including lots of great functionality from authors such as Eric Hexter and
Derek Greer.  

Should - Write cleaner asserts using the Should library, including SpecsFor's many custom Should extensions.
Looks - Thanks to ExpectedObjects, you can easily compare that two objects *look* like one another.
Mocking - Mocking is provided by Moq, but many common pain points are alleviated by SpecsFor's extensions.
Conventions - Customize SpecsFor's behavior by creating a custom SpecsForConfiguration class.

If there's a common testing pattern, utility, or library that you think should be bundled with SpecsFor, let me know!

-------
Credits
-------
My thanks go to the authors of libraries that SpecsFor utilizes:
-ExpectedObjects
-Moq
-NUnit
-Should
-StructureMap.AutoMocking
