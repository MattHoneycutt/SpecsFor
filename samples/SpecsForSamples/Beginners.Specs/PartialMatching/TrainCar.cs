namespace Beginners.PartialMatching
{
	public class TrainCar
	{
		public Person Conductor { get; set; }

		public string Name { get; set; }

		public int MaxPassengers { get; set; }

		public int YearBuilt { get; set; }
	}

	public class Person
	{
		public string FirstName { get; set; }

		public string LastName { get; set; }
	}
}