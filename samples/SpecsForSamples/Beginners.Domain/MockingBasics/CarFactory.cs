namespace Beginners.Domain.MockingBasics
{
	public class CarFactory
	{
		private readonly IEngineFactory _engineFactory;

		public CarFactory(IEngineFactory engineFactory)
		{
			_engineFactory = engineFactory;
		}

		public Car BuildMuscleCar()
		{
			return new Car(_engineFactory.GetEngine("V8"));
		}
	}
}