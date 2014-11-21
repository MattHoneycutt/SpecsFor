namespace Beginners.Domain.MockingBasics
{
	public interface IEngineFactory
	{
		Engine GetEngine(string engineType);
	}
}