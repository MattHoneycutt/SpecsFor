using SpecsFor.Core;
using SpecsFor.Core.Configuration;

namespace SpecsFor.Lamar.Tests.ComposingContext.TestDomain;

public class ProvideMagicByTypeName : Behavior<ISpecs>
{
    public override void Given(ISpecs instance)
    {
        ((ILikeMagic)instance).CalledByDuringGiven.Add(GetType().Name);
    }
		
    public override void AfterGiven(ISpecs instance)
    {
        ((ILikeMagic)instance).CalledByAfterGiven.Add(GetType().Name);
    }

    public override void AfterSpec(ISpecs instance)
    {
        ((ILikeMagic)instance).CalledByAfterTest.Add(GetType().Name);
    }
}