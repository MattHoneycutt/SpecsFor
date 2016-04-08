namespace Beginners.Domain.MockingBasics
{
	public class Engine
	{
	    public int YearBuilt { get; set; }
	    public string Maker { get; set; }
	    public string Type { get; set; }
	    public bool IsStopped { get; set; }

	    public Engine()
		{
			IsStopped = true;
		}

		public void Start()
		{
			IsStopped = false;
		}

		public void Stop()
		{
			IsStopped = true;
		}
	}
}