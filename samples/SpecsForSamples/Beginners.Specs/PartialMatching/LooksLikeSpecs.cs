using System;
using Moq;
using NUnit.Framework;
using SpecsFor;
using SpecsFor.ShouldExtensions;

namespace Beginners.PartialMatching
{
	public class LooksLikeSpecs
	{
		public class when_verifying_with_a_partial_object : SpecsFor<object>
		{
			[Test]
			public void then_it_verifies_correctly_if_the_object_matches_the_specified_properties()
			{
				var myCar = new TrainCar {Name = "Simple Car", MaxPassengers = 100, YearBuilt = 2014};

				GetMockFor<ITrainYard>().Object
					.StoreCar(myCar);

				GetMockFor<ITrainYard>()
					.Verify(x => x.StoreCar(Looks.Like(() => new TrainCar{YearBuilt = 2014})));
			}

			[Test]
			public void then_it_verifies_correctly_if_the_matcher_is_satisfied()
			{
				GetMockFor<ITrainYard>().Object
					.RetrieveLuggage(new LuggageTicket
					{
						IssuedTo = "Jane Doe",
						Issued = DateTime.Now,
					});

				GetMockFor<ITrainYard>()
					.Verify(x => x.RetrieveLuggage(Looks.Like(() => new LuggageTicket
					{
						IssuedTo = "Jane Doe",
						Issued = Some.ValueOf<DateTime>(d => DateTime.Now.Subtract(d) < TimeSpan.FromSeconds(1))
					})));
			}
		}
	}
}