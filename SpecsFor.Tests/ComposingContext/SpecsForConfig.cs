using System;
using System.Collections.Generic;
using NUnit.Framework;
using SpecsFor.Configuration;

namespace SpecsFor.Tests.ComposingContext
{
	[SetUpFixture]
	public class SpecsForConfig
	{
		[SetUp]
		public void SetupTestRun()
		{
			SpecsForBehaviors.Configure(cfg =>
				{
					cfg.WhenTesting<ILikeMagic>().EnrichWith<ProvideMagicByInterface>();
					cfg.WhenTesting<SpecsFor<Widget>>().EnrichWith<ProvideMagicByConcreteType>();
					cfg.WhenTesting(t => t.Name.Contains("running_tests_decorated")).EnrichWith<ProvideMagicByTypeName>();
					cfg.WhenTesting(t => t.Name.Contains("junk that does not exist")).EnrichWith<DoNotProvideMagic>();
					cfg.WhenTestingAnything().EnrichWith<ProvideMagicForEveryone>();
					//May or may not need this? This could be a way to say "for any class that is a spec for T," regardless
					//of the actual spec class's type.  This would allow it to match even custom SpecsFor types. 
					//cfg.ForSpecs<IRequireContext>().EnrichWith<SomethingElse>();
				});
		}
	}

	public class DoNotProvideMagic : IBehavior<object>
	{
		public void Given(object instance)
		{
			((ILikeMagic)instance).CalledByDuringGiven.Add(GetType().Name);
		}
	}

	public class ProvideMagicByTypeName : IBehavior<object>
	{
		public void Given(object instance)
		{
			((ILikeMagic)instance).CalledByDuringGiven.Add(GetType().Name);
		}
	}

	public class ProvideMagicForEveryone : IBehavior<object>
	{
		public void Given(object instance)
		{
			((ILikeMagic) instance).CalledByDuringGiven.Add(GetType().Name);
		}
	}

	public class Widget : ILikeMagic
	{
		public List<string> CalledByDuringGiven { get; set; }
		public List<string> CalledByAfterTest { get; set; }

		public Widget()
		{
			CalledByDuringGiven = new List<string>();
			CalledByAfterTest = new List<string>();
		}
	}

	public interface ILikeMagic
	{
		List<string> CalledByDuringGiven { get; set; }
		List<string> CalledByAfterTest { get; set; }
	}

	public class ProvideMagicByInterface : IBehavior<ILikeMagic>
	{
		public void Given(ILikeMagic instance)
		{
			instance.CalledByDuringGiven.Add(GetType().Name);
		}
	}

	public class ProvideMagicByConcreteType : IBehavior<SpecsFor<Widget>>
	{
		public void Given(SpecsFor<Widget> instance)
		{
			((ILikeMagic)instance).CalledByDuringGiven.Add(GetType().Name);
		}
	}
}