using System;
using System.Diagnostics;

namespace SpecsFor.StructureMap.Tests.ComposingContext.TestDomain
{
	//TODO: Put in the demo project
	public class MyTestLogger : Behavior<ISpecs>
	{
		private Stopwatch _stopwatch;

		public override void Given(ISpecs instance)
		{
			_stopwatch = Stopwatch.StartNew();
		}

		public override void AfterSpec(ISpecs instance)
		{
			Console.WriteLine(_stopwatch.Elapsed.TotalSeconds);
		}
	}
}