using System.Collections.Generic;

namespace SpecsFor.Tests.ComposingContext.TestDomain
{
	public interface ILikeMagic
	{
		List<string> CalledByDuringGiven { get; set; }
		List<string> CalledByAfterTest { get; set; }
	}
}