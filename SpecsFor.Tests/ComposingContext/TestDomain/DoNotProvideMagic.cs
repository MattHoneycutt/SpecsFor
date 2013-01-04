using SpecsFor.Configuration;

namespace SpecsFor.Tests.ComposingContext.TestDomain
{
	public class DoNotProvideMagic : Behavior<ISpecs>
	{
		public override void Given(ISpecs instance)
		{
			((ILikeMagic)instance).CalledByDuringGiven.Add(GetType().Name);
		}

		public override void AfterSpec(ISpecs instance)
		{
			((ILikeMagic)instance).CalledByAfterTest.Add(GetType().Name);
		}
	}
}