using NUnit.Framework;
using SpecsFor;
using SpecsFor.ShouldExtensions;

namespace Beginners.PartialMatching
{
	public class PartialMatchingWithSome
	{
		public class when_using_some_compare_properties : SpecsFor<object>
		{
			[Test]
			public void then_it_passes_if_the_property_matches_the_expectation()
			{
				var train = new TrainCar {MaxPassengers = 123};

				train.ShouldLookLike(() => new TrainCar
				{
					MaxPassengers = Some.ValueOf<int>(x => x > 100)
				});
			}

			[Test]
			public void then_it_passes_if_the_property_falls_within_the_expected_range()
			{
				var train = new TrainCar {MaxPassengers = 123};

				train.ShouldLookLike(() => new TrainCar
				{
					MaxPassengers = Some.ValueInRange(120, 130)
				});
			}

			[Test]
			public void then_it_throws_if_the_property_falls_outside_the_range()
			{
				var train = new TrainCar { MaxPassengers = 123 };

				train.ShouldLookLike(() => new TrainCar
				{
					MaxPassengers = Some.ValueInRange(100, 120)
				});
			}
		}
	}
}