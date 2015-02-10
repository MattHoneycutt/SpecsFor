namespace SpecsFor.Mvc.Demo.Areas.Tasks.Models
{
    public class Task
    {
        public string Title { get; set; }

        public bool Complete { get; set; }

	    public Priority Priority { get; set; }
    }

	public enum Priority
	{
		Normal = 0,
		Low = 1,
		High = 2
	}
}