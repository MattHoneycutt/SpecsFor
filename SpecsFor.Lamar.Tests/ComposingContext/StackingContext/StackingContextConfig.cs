using NUnit.Framework;
using SpecsFor.Core.Configuration;
using SpecsFor.Lamar.Tests.ComposingContext.TestDomain;

namespace SpecsFor.Lamar.Tests.ComposingContext.StackingContext;

[SetUpFixture]
public class StackingContextConfig : SpecsForConfiguration
{
    public StackingContextConfig()
    {
        WhenTesting<ILikeMagic>().EnrichWith<NestedMagicProvider>();
    }
}

public class NestedMagicProvider : Behavior<ILikeMagic>
{
    public override void Given(ILikeMagic instance)
    {
        instance.CalledByDuringGiven.Add(this.GetType().Name);
    }
		
    public override void AfterGiven(ILikeMagic instance)
    {
        instance.CalledByAfterGiven.Add(this.GetType().Name);
    }

    public override void AfterSpec(ILikeMagic instance)
    {
        instance.CalledByAfterSpec.Add(this.GetType().Name);
    }
    
    public override void AfterTest(ILikeMagic instance)
    {
        instance.CalledByAfterSpec.Add(this.GetType().Name);
    }
		
    public override void BeforeTest(ILikeMagic instance)
    {
        instance.CalledByBeforeTest.Add(this.GetType().Name);
    }
}