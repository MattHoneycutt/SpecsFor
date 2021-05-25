using SpecsFor.Core.Configuration;

namespace SpecsFor.StructureMap.Tests.ComposingContext.TestDomain
{
	public class ProvideMagicByInterface : Behavior<ILikeMagic>
	{
		public override void Given(ILikeMagic instance)
		{
			instance.CalledByDuringGiven.Add(GetType().Name);
		}
		
		public override void AfterGiven(ILikeMagic instance)
		{
			instance.CalledByAfterGiven.Add(GetType().Name);
		}

		public override void AfterSpec(ILikeMagic instance)
		{
			instance.CalledByAfterTest.Add(GetType().Name);
		}
	}
}