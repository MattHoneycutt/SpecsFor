using SpecsFor.Configuration;

namespace SpecsFor.Autofac.Tests.ComposingContext.TestDomain
{
	public class ProvideMagicByInterface : Behavior<ILikeMagic>
	{
		public override void Given(ILikeMagic instance)
		{
			instance.CalledByDuringGiven.Add(GetType().Name);
		}

		public override void AfterSpec(ILikeMagic instance)
		{
			instance.CalledByAfterTest.Add(GetType().Name);
		}
	}
}