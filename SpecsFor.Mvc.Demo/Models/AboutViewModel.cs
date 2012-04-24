namespace SpecsFor.Mvc.Demo.Models
{
	public class AboutViewModel
	{
		public string DayOfWeek { get; set; }

		public UserViewModel User { get; set; }

		public string[] BusinessDays { get; set; }
	}
}