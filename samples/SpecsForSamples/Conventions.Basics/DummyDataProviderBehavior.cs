using System;
using System.Collections.Generic;
using Conventions.Basics.Domain;
using SpecsFor;
using SpecsFor.Configuration;

namespace Conventions.Basics
{
    public interface INeedDummyData : ISpecs
    {
        IEnumerable<Foo> Foos { get; set; }
    }

    public class DummyDataProviderBehavior : Behavior<INeedDummyData>
    {
        public override void Given(INeedDummyData instance)
        {
            instance.Foos = new[]
            {
                new Foo {Id = Guid.NewGuid(), Name = "Foo 1"},
                new Foo {Id = Guid.NewGuid(), Name = "Foo 2"},
                new Foo {Id = Guid.NewGuid(), Name = "Foo 3"},
                new Foo {Id = Guid.NewGuid(), Name = "Foo 4"},
                new Foo {Id = Guid.NewGuid(), Name = "Foo 5"},
            };
        }
    }
}