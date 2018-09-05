using Moq;
using SpecsFor.Core;

namespace SpecsFor.StructureMap
{
    public class StructureMapAutoMocker<TSut> : IAutoMocker where TSut : class
    {
        private readonly MoqAutoMocker<TSut> _internalAutoMocker;

        public StructureMapAutoMocker()
        {
            _internalAutoMocker = new MoqAutoMocker<TSut>();
        }

        T IAutoMocker.CreateSUT<T>()
        {
            return _internalAutoMocker.ClassUnderTest as T;
        }

        public Mock<T> GetMockFor<T>() where T : class
        {
            return Mock.Get(_internalAutoMocker.Get<T>());
        }

        public void ConfigureContainer<T>(ISpecs<T> specsFor) where T : class
        {
            var specs = (SpecsFor<TSut>) specsFor;

            specs.ConfigureContainer(_internalAutoMocker.Container);
        }
    }
}