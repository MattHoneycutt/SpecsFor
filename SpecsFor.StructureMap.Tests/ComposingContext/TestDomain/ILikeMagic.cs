using System.Collections.Generic;

namespace SpecsFor.StructureMap.Tests.ComposingContext.TestDomain
{
	public interface ILikeMagic
	{
		List<string> CalledByDuringGiven { get; set; }
		List<string> CalledByAfterGiven { get; set; }
		List<string> CalledByAfterSpec { get; set; }
		List<string> CalledByAfterTest { get; set; }
		List<string> CalledByBeforeTest { get; set; }
		List<string> CalledByApplyAfterClassUnderTestInitialized { get; set; }
		List<string> CalledBySpecInit { get; set; }
	}
}