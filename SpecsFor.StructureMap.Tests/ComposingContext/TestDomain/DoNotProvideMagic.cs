using SpecsFor.Core;
using SpecsFor.Core.Configuration;

namespace SpecsFor.StructureMap.Tests.ComposingContext.TestDomain
{
	public class DoNotProvideMagic : Behavior<ISpecs>
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
			((ILikeMagic)instance).CalledByAfterSpec.Add(GetType().Name);
		}
		
		public override void AfterTest(ISpecs instance)
		{
			((ILikeMagic)instance).CalledByAfterTest.Add(GetType().Name);
		}
    
		public override void BeforeTest(ISpecs instance)
		{
			((ILikeMagic)instance).CalledByBeforeTest.Add(GetType().Name);
		}
	}
}