using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Linq;

namespace SpecsFor.StructureMap;

public class SpecsForAutoMocker<TSut> where TSut : class
{
    public Container Container { get; protected set; }

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
        Container = new Container(config =>
        {
            config.For<TSut>().Use<TSut>();

            var constructor = typeof(TSut).GetConstructors()
                                          .OrderByDescending(c => c.GetParameters().Length)
                                          .FirstOrDefault();

            if (constructor == null)
            {
                return;
            }

            // Register dependencies
            foreach (var parameter in constructor.GetParameters()
                         .Where(x => x.ParameterType is { IsValueType: false, IsPointer: false }))
            {
                var parameterType = parameter.ParameterType;
                var mockType = typeof(Mock<>).MakeGenericType(parameterType);
                var mockInstance = Activator.CreateInstance(mockType);
                var instanceValue = mockType.GetProperties()
                                            .First(x => x.Name == nameof(Mock.Object))
                                            .GetValue(mockInstance);

                config.AddTransient(parameterType, _ => instanceValue);
            }
        });
    }
}