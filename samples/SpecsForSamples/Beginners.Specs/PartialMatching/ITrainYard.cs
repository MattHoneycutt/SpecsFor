namespace Beginners.PartialMatching
{
	public interface ITrainYard
	{
		void StoreCar(TrainCar trainCar);
		void RetrieveLuggage(LuggageTicket ticket);
	}
}