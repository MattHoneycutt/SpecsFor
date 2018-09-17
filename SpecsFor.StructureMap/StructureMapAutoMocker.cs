using Moq;
using SpecsFor.Core;

namespace SpecsFor.StructureMap
{
    public class StructureMapAutoMocker<TSut> : IAutoMocker where TSut : class
    {
        public MoqAutoMocker<TSut> MoqAutoMocker { get; }

        public StructureMapAutoMocker()
        {
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

        public void ConfigureContainer<T>(ISpecs<T> specsFor) where T : class
        {
            var specs = (SpecsFor<TSut>) specsFor;

            specs.ConfigureContainer(MoqAutoMocker.Container);
        }
    }
}