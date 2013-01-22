using SpecsFor.Configuration;

namespace SpecsFor.Tests.ComposingContext.TestDomain
{
	public class ProvideMagicForEveryone : Behavior<ISpecs>
	{
		public override void Given(ISpecs instance)
		{
			((ILikeMagic) instance).CalledByDuringGiven.Add(GetType().Name);
		}

		public override void AfterSpec(ISpecs instance)
		{
			((ILikeMagic)instance).CalledByAfterTest.Add(GetType().Name);
		}

		public override void ClassUnderTestInitialized(ISpecs instance)
		{
			((ILikeMagic)instance).CalledByApplyAfterClassUnderTestInitialized.Add(GetType().Name);
		}

		public override void SpecInit(ISpecs instance)
		{
			((ILikeMagic)instance).CalledBySpecInit.Add(GetType().Name);
		}
	}
}