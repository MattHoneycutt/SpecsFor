using System.Collections.Generic;

namespace SpecsFor.Autofac.Tests.ComposingContext.TestDomain
{
	public class Widget : ILikeMagic
	{
		public List<string> CalledByDuringGiven { get; set; }
		public List<string> CalledByAfterTest { get; set; }
		public List<string> CalledByApplyAfterClassUnderTestInitialized { get; set; }
		public List<string> CalledBySpecInit { get; set; }

		public Widget()
		{
			CalledBySpecInit = new List<string>();
			CalledByApplyAfterClassUnderTestInitialized = new List<string>();
			CalledByDuringGiven = new List<string>();
			CalledByAfterTest = new List<string>();
		}
	}
}