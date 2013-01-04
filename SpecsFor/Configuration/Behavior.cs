namespace SpecsFor.Configuration
{
	//QUESTION: Abstract base class with template methods?  Or marker interface
	//			that applies context based on method names?  
	public abstract class Behavior<T> where T : class
	{
		public virtual void Given(T instance)
		{
		}

		public virtual void AfterSpec(T instance)
		{
		}
	}
}