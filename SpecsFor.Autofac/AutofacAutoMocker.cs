using Moq;
using Autofac.Extras.Moq;
using SpecsFor.Core;

namespace SpecsFor.Autofac
{
    public class AutofacAutoMocker<TSut> : IAutoMocker where TSut : class
    {
        public AutoMock InternalMocker { get; private set; }

        private readonly SpecsFor<TSut> _specsFor;

        public AutofacAutoMocker(ISpecs<TSut> specsFor)
        {
            _specsFor = (SpecsFor<TSut>)specsFor;

            InternalMocker = _specsFor.CreateInternalMocker();
        }

        public TSut CreateSUT<TSut>() where TSut : class
        {
            return InternalMocker.Create<TSut>();
        }

        public Mock<T> GetMockFor<T>() where T : class
        {
            return InternalMocker.Mock<T>();
        }

        public void ConfigureContainer()
        {
            _specsFor.ConfigureMocker(InternalMocker);
        }
    }
}