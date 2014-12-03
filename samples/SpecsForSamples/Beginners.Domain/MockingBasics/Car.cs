namespace Beginners.Domain.MockingBasics
{
	public class Car
	{
		public Engine Engine { get; set; }

		public bool IsStopped { get; set; }

		public Car(Engine engine)
		{
			Engine = engine;
			IsStopped = true;
		}

		public void Start()
		{
			Engine.Start();
			IsStopped = false;
		}

		public void Stop()
		{
			Engine.Stop();
			IsStopped = true;
		}
	}
}