using System;

namespace Conventions.Basics.Domain
{
    public class FooFactory
    {
        public Foo Create(string name = "Unnamed")
        {
            return new Foo
            {
                Id = Guid.NewGuid(),
                Name = name
            };
        }
    }
}