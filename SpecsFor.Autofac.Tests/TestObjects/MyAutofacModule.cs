using Autofac;

namespace SpecsFor.Autofac.Tests.TestObjects
{
    public class MyAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SpecialFooTimesTen>().As<IFoo>();
        }
    }

    public class SpecialFooTimesTen : IFoo
    {
        public int Bar(int x)
        {
            return x * 10;
        }
    }
}