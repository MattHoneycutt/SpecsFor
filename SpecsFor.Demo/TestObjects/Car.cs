namespace SpecsFor.Tests.TestObjects
{
	public class Car
	{
		private readonly IEngine _engine;
		private readonly ITransmission _transmission;

		public Car(IEngine engine, ITransmission transmission)
		{
			_engine = engine;
			_transmission = transmission;
		}

		public void TurnKey()
		{
			_engine.Start();
		}
	}

	public interface ITransmission
	{
		string Gear { get; set; }
	}

	public interface IEngine
	{
		void Start();
	}
}