using Beginners.Domain;
using Beginners.Domain.MockingBasics;
using NUnit.Framework;
using SpecsFor;
using SpecsFor.ShouldExtensions;

namespace PartialMatching
{
    public class FuzzyMatchingExampleSpecs
    {
        public class when_you_only_care_about_what_things_look_like : SpecsFor<object>
        {
            [Test]
            public void then_you_can_check_for_a_non_null_value()
            {
                var engine = new Engine { Maker = "Heroic" };
                engine.ShouldLookLike(() => new Engine
                {
                    Maker = Any.NonNullValueOf<string>()
                });
            }

            [Test]
            public void then_you_can_check_for_a_value_in_a_range()
            {
                var engine = new Engine {YearBuilt = 2015};
                engine.ShouldLookLike(() => new Engine
                {
                    YearBuilt = Some.ValueInRange(2014, 2016)
                });
            }

            [Test]
            public void then_you_can_check_for_a_value_matching_some_expression()
            {
                var engine = new Engine {Maker = "Heroic"};
                engine.ShouldLookLike(() => new Engine
                {
                    Maker = Some.ValueOf<string>(s => char.IsUpper(s[0]))
                });
            }

            [Test]
            public void then_you_can_check_for_an_item_in_a_list()
            {
                var warehouse = new Warehouse
                {
                    Engines = new[] {new Engine {YearBuilt = 2013}, new Engine {YearBuilt = 2016}}
                };

                warehouse.ShouldLookLike(() => new Warehouse
                {
                    Engines = Some.ListContaining(() => new Engine
                    {
                        //Yep, you can use partial matching recursively!
                        YearBuilt = Some.ValueOf<int>(i => i % 2 == 0)
                    })
                });
            }
        }
    }
}