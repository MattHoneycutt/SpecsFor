using SpecsFor.Core.Configuration;

namespace SpecsFor.StructureMap.Tests.ComposingContext.TestDomain
{
	public class ProvideMagicByConcreteType : Behavior<SpecsFor<Widget>>
	{
		public override void Given(SpecsFor<Widget> instance)
		{
			((ILikeMagic)instance).CalledByDuringGiven.Add(GetType().Name);
		}
		
		public override void AfterGiven(SpecsFor<Widget> instance)
		{
			((ILikeMagic)instance).CalledByAfterGiven.Add(GetType().Name);
		}

		public override void AfterSpec(SpecsFor<Widget> instance)
		{
			((ILikeMagic)instance).CalledByAfterTest.Add(GetType().Name);
		}
	}
}