namespace Beginners.Domain.MockingBasics
{
	public class RealEngineFactory : IEngineFactory
	{
		public Engine GetEngine(string engineType)
		{
			return new Engine
			{
				Maker = "Real Engines, Inc",
				Type = engineType
			};
		}
	}
}