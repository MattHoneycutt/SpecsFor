using System;
using System.Diagnostics;
using SpecsFor;
using SpecsFor.Configuration;

namespace Conventions.Basics
{
    public class LogExecutionTimeBehavior : Behavior<ISpecs>
    {
        private Stopwatch _stopwatch;

        public override void SpecInit(ISpecs instance)
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public override void AfterSpec(ISpecs instance)
        {
            _stopwatch.Stop();

            Console.WriteLine($"{instance.GetType().Name} - {_stopwatch.Elapsed}");
        }
    }
}