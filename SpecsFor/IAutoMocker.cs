using Moq;

namespace SpecsFor.Core
{
    public interface IAutoMocker
    {
        TSut CreateSUT<TSut>() where TSut : class;

        Mock<T> GetMockFor<T>() where T : class;

        void ConfigureContainer<TSut>(ISpecs<TSut> specsFor) where TSut : class;
    }
}