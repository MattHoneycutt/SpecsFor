using NUnit.Framework;
using SpecsFor.Configuration;
using SpecsFor.Tests.ComposingContext.TestDomain;

namespace SpecsFor.Tests.ComposingContext
{
	[SetUpFixture]
	public class ComposingContextConfig : SpecsForConfiguration
	{
		public ComposingContextConfig()
		{
			WhenTesting<ILikeMagic>().EnrichWith<ProvideMagicByInterface>();
			WhenTesting<SpecsFor<Widget>>().EnrichWith<ProvideMagicByConcreteType>();
			WhenTesting(t => t.Name.Contains("running_tests_decorated")).EnrichWith<ProvideMagicByTypeName>();
			WhenTesting(t => t.Name.Contains("junk that does not exist")).EnrichWith<DoNotProvideMagic>();
			WhenTestingAnything().EnrichWith<ProvideMagicForEveryone>();
			WhenTestingAnything().EnrichWith<MyTestLogger>();
			//May or may not need this? This could be a way to say "for any class that is a spec for T," regardless
			//of the actual spec class's type.  This would allow it to match even custom SpecsFor types. 
			//cfg.ForSpecsOn<IRequireContext>().EnrichWith<SomethingElse>();
		}
	}
}