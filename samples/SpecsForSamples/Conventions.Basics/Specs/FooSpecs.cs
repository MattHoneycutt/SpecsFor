using System.Collections.Generic;
using Conventions.Basics.Domain;
using NUnit.Framework;
using Should;
using SpecsFor;

namespace Conventions.Basics.Specs
{
    public class FooSpecs
    {
        public class when_a_spec_has_the_right_marker_interface 
            : SpecsFor<Foo>, INeedDummyData
        {
            public IEnumerable<Foo> Foos { get; set; }

            [Test]
            public void then_it_injects_some_foos()
            {
                Foos.ShouldNotBeEmpty();
            }
        }
    }
}