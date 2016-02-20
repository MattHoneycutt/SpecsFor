using Conventions.Basics.Domain;
using SpecsFor;
using SpecsFor.Configuration;

namespace Conventions.Basics
{
    public interface INeedMockHelloService : ISpecs
    {
    }

    public class MockHelloServiceBehavior : Behavior<INeedMockHelloService>
    {
        public override void SpecInit(INeedMockHelloService instance)
        {
            instance.GetMockFor<IHelloService>()
                .Setup(x => x.SayHello())
                .Returns("Hello from Moq instance!");
        }
    }
}