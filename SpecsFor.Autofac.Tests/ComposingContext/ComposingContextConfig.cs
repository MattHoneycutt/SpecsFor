using NUnit.Framework;
using SpecsFor.Autofac.Tests.ComposingContext.TestDomain;
using SpecsFor.Core.Configuration;

namespace SpecsFor.Autofac.Tests.ComposingContext
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
		}
	}
}