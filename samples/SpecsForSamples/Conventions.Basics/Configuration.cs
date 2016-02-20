using NUnit.Framework;
using SpecsFor;
using SpecsFor.Configuration;

namespace Conventions.Basics
{
    [SetUpFixture]
    public class Configuration : SpecsForConfiguration
    {
        public Configuration()
        {
            WhenTestingAnything().EnrichWith<LogExecutionTimeBehavior>();

            WhenTesting<INeedDummyData>().EnrichWith<DummyDataProviderBehavior>();

            WhenTesting<INeedMockHelloService>().EnrichWith<MockHelloServiceBehavior>();
            WhenTesting<INeedRealHelloService>().EnrichWith<RealHelloServiceBehavior>();

            WhenTesting<INeedATransaction>().EnrichWith<TransactionScopeWrapperBehavior>();
        }
    }
}
