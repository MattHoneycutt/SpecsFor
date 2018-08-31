namespace SpecsFor.Autofac.Tests.TestObjects
{
    public class MyTestSut
    {
        private readonly IFoo _foo;

        public MyTestSut(IFoo foo)
        {
            _foo = foo;
        }

        public int TimesTwo(int x)
        {
            return _foo.Bar(x) * 2;
        }
    }
}