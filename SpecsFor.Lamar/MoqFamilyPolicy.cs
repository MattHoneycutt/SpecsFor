using System.Collections.Concurrent;
using Lamar;
using Lamar.IoC.Instances;
using Moq;

namespace SpecsFor.Lamar;

internal sealed class MoqFamilyPolicy : IFamilyPolicy
{
    private readonly ConcurrentDictionary<Type, object> _cache = new();

    public ServiceFamily Build(Type type, ServiceGraph graph)
    {
        if (!type.IsInterface && !type.IsAbstract) return null;

        var mock = GetOrAddMockFromCache(type);

        var objectInstance = ObjectInstance.For(mock.Object); 
        return new ServiceFamily(type, graph.DecoratorPolicies, objectInstance);
    }

    private Mock GetOrAddMockFromCache(Type type)
    {
        var mock = (Mock)_cache.GetOrAdd(type, t =>
        {
            var generic = typeof(Mock<>).MakeGenericType(t);
            return Activator.CreateInstance(generic)!;
        });
        return mock;
    }

    public Mock<T> GetMock<T>() where T : class => GetOrAddMockFromCache(typeof(T)) as Mock<T> ?? 
                                                   throw new InvalidOperationException($"No mock found for type {typeof(T).FullName}.");
}
