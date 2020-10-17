using NUnit.Framework;
using SpecsFor.Autofac.Tests.ComposingContext.TestDomain;
using SpecsFor.Core.Configuration;

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
			instance.CalledByDuringGiven.Add(GetType().Name);
		}

		public override void AfterSpec(ILikeMagic instance)
		{
			instance.CalledByAfterTest.Add(GetType().Name);
		}
	}
}