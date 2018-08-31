using NUnit.Framework;
using SpecsFor.Autofac.Tests.ComposingContext.TestDomain;
using SpecsFor.Configuration;

namespace SpecsFor.Autofac.Tests.ComposingContext.StackingContext
{
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

		public override void AfterSpec(ILikeMagic instance)
		{
			instance.CalledByAfterTest.Add(this.GetType().Name);
		}
	}
}