namespace SpecsFor.Core.Configuration
{
	public abstract class Behavior<T> where T : class
	{
		public virtual void SpecInit(T instance)
		{
		}

		public virtual void ClassUnderTestInitialized(T instance)
		{
		}

		public virtual void Given(T instance)
		{
		}

		public virtual void AfterGiven(T instance)
		{
		}

		public virtual void AfterSpec(T instance)
		{
		}
	}
}