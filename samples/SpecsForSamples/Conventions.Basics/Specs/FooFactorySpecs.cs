using System;
using Conventions.Basics.Domain;
using NUnit.Framework;
using Should;
using SpecsFor;

namespace Conventions.Basics.Specs
{
    public class FooFactorySpecs
    {
        public class when_creating_a_foo_with_no_options : SpecsFor<FooFactory>
        {
            private Foo _result;

            protected override void When()
            {
                _result = SUT.Create();
            }

            [Test]
            public void then_it_uses_the_right_default_name()
            {
                _result.Name.ShouldEqual("Unnamed");
            }

            [Test]
            public void then_it_assigns_an_id()
            {
                _result.Id.ShouldNotEqual(Guid.Empty);
            }
        }

        public class when_creating_a_foo_with_a_name : SpecsFor<FooFactory>
        {
            private Foo _result;

            protected override void When()
            {
                _result = SUT.Create("TestFoo");
            }

            [Test]
            public void then_it_uses_the_specified_name()
            {
                _result.Name.ShouldEqual("TestFoo");
            }

            [Test]
            public void then_it_assigns_an_id()
            {
                _result.Id.ShouldNotEqual(Guid.Empty);
            }
        }
    }
}