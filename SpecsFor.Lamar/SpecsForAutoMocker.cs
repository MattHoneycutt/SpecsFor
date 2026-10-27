using Lamar;
using Moq;

namespace SpecsFor.Lamar;

public class SpecsForAutoMocker<TSut> where TSut : class
{
    private readonly MoqFamilyPolicy _policy = new();
    public Container Container { get; }

    public SpecsForAutoMocker(Action<ServiceRegistry> extraConfig = null)
    {
        var services = new ServiceRegistry();
        services.For<TSut>().Use<TSut>();

        services.Policies.OnMissingFamily(_policy);
        
        extraConfig?.Invoke(services);
        Container = new Container(services);
    }

    public TSut ClassUnderTest => Container.GetInstance<TSut>();
    
    public Mock<T> GetMock<T>() where T : class => _policy.GetMock<T>();
}
