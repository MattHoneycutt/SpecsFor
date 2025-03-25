using System.Diagnostics;
using SpecsFor.Core;
using SpecsFor.Core.Configuration;

namespace SpecsFor.Lamar.Tests.ComposingContext.TestDomain;

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