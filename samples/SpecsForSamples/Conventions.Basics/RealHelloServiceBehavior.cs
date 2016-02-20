using Conventions.Basics.Domain;
using SpecsFor;
using SpecsFor.Configuration;

namespace Conventions.Basics
{
    public interface INeedRealHelloService : ISpecs
    {
    }

    public class RealHelloServiceBehavior : Behavior<INeedRealHelloService>
    {
        public override void SpecInit(INeedRealHelloService instance)
        {
            instance.MockContainer.Inject<IHelloService>(new HelloService());
        }
    }
}