using System;
using System.Diagnostics;
using SpecsFor.Core;
using SpecsFor.Core.Configuration;

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

		public override void AfterTest(ISpecs instance)
		{
			Console.WriteLine("After test");
		}
		
		public override void BeforeTest(ISpecs instance)
		{
			Console.WriteLine("Before test");
		}
	}
}