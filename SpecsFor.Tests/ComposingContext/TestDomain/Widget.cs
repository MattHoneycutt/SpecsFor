using System.Collections.Generic;

namespace SpecsFor.Tests.ComposingContext.TestDomain
{
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
}