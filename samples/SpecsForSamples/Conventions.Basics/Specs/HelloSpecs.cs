using Conventions.Basics.Domain;
using NUnit.Framework;
using Should;
using SpecsFor;

namespace Conventions.Basics.Specs
{
    public class HelloSpecs
    {
        public class when_using_the_real_hello_service 
            : SpecsFor<HelloConsumer>, INeedRealHelloService
        {
            [Test]
            public void then_it_returns_the_real_string()
            {
                SUT.GetHelloMessage().ShouldEqual("Hello from HelloService!");
            }
        }

        public class when_using_the_mock_hello_service 
            : SpecsFor<HelloConsumer>, INeedMockHelloService
        {
            [Test]
            public void then_it_returns_the_mock_string()
            {
                SUT.GetHelloMessage().ShouldEqual("Hello from Moq instance!");
            }
        }
    }
}