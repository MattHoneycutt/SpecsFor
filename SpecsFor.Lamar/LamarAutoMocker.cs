using Moq;
using SpecsFor.Core;

namespace SpecsFor.Lamar;

public class LamarAutoMocker<TSut> : IAutoMocker where TSut : class
{
    public SpecsForAutoMocker<TSut> MoqAutoMocker { get; }

    private readonly SpecsFor<TSut> _specsFor;

    public LamarAutoMocker(ISpecs<TSut> specsFor)
    {
        _specsFor = (SpecsFor<TSut>)specsFor;

        MoqAutoMocker = new SpecsForAutoMocker<TSut>();
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