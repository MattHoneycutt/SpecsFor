using Moq;
using SpecsFor.Core;

namespace SpecsFor.StructureMap
{
    public class StructureMapAutoMocker<TSut> : IAutoMocker where TSut : class
    {
        public MoqAutoMocker<TSut> MoqAutoMocker { get; }

        private readonly SpecsFor<TSut> _specsFor;

        public StructureMapAutoMocker(ISpecs<TSut> specsFor)
        {
            _specsFor = (SpecsFor<TSut>)specsFor;

            // TODO: Create MoqAutoMocker class that doesn't implement AutoMocker?
            MoqAutoMocker = new MoqAutoMocker<TSut>();
        }

        T IAutoMocker.CreateSUT<T>()
        {
            return MoqAutoMocker.ClassUnderTest as T;
        }

        public Mock<T> GetMockFor<T>() where T : class
        {
            return Mock.Get(MoqAutoMocker.Get<T>());
        }

        public void ConfigureContainer()
        {
            _specsFor.ConfigureContainer(MoqAutoMocker.Container);
        }
    }
}