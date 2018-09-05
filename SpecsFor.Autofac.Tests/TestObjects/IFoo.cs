namespace SpecsFor.Autofac.Tests.TestObjects
{
    public interface IFoo
    {
        int Bar(int x);
    }

    public class Foo : IFoo 
    {
        public int Bar(int x)
        {
            return x * 3;
        }
    }
}