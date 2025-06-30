using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Reflection;

namespace SpecsFor.Lamar;

public class SpecsForAutoMocker<TSut> where TSut : class
{
    public Container Container { get; protected set; }

    private readonly HashSet<Type> _registeredTypes = new();

    public T Get<T>() where T : class
    {
        var instance = Container.TryGetInstance<T>();

        if (instance != null)
        {
            return instance;
        }

        var mockedInstance = Mock.Of<T>();

        Container.Configure(x => x.AddTransient(typeof(T), _ => mockedInstance));

        return mockedInstance;
    }

    public TSut ClassUnderTest => Container.GetInstance<TSut>();

    public SpecsForAutoMocker()
    {
        var registry = new ServiceRegistry();
        RegisterType(typeof(TSut), registry);
        Container = new Container(registry);
    }

    private void RegisterType(Type type, ServiceRegistry registry)
    {
        if (_registeredTypes.Contains(type))
        {
            return;
        }

        _registeredTypes.Add(type);

        if (type.IsInterface || type.IsAbstract)
        {
            var mockType = typeof(Mock<>).MakeGenericType(type);
            var mockInstance = Activator.CreateInstance(mockType)!;

            var mockObject = mockType
                .GetProperty("Object",
                    BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)!
                .GetValue(mockInstance);
            registry.AddTransient(type, _ => mockObject);
            return;
        }

        if (type.IsPrimitive || type == typeof(string))
        {
            registry.AddTransient(type, _ => Activator.CreateInstance(type));
            return;
        }

        var constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (constructor == null)
        {
            // No public constructor found, cannot register type
            return;
        }

        var parameters = constructor.GetParameters();

        foreach (var param in parameters)
        {
            RegisterType(param.ParameterType, registry);
        }

        registry.For(type).Use(type);
    }
}