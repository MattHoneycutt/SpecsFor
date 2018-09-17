using Moq;
using Autofac.Extras.Moq;
using SpecsFor.Core;

namespace SpecsFor.Autofac
{
    public class AutofacAutoMocker : IAutoMocker
    {
        public AutoMock Mocker { get; private set; }

        public TSut CreateSUT<TSut>() where TSut : class
        {
            return Mocker.Create<TSut>();
        }

        public Mock<T> GetMockFor<T>() where T : class
        {
            return Mocker.Mock<T>();
        }

        public void ConfigureContainer<TSut>(ISpecs<TSut> specsFor) where TSut : class
        {
            var specs = (SpecsFor<TSut>) specsFor;

            Mocker = specs.CreateMocker();

            specs.ConfigureMocker(Mocker);
        }
    }
}