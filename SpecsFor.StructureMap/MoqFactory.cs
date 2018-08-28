using System;
using System.Reflection;

namespace SpecsFor.StructureMap
{
    public class MoqFactory
    {
        private readonly Type _mockOpenType;

        public MoqFactory()
        {
            var moq = Assembly.Load("Moq");
            _mockOpenType = moq.GetType("Moq.Mock`1");
            if (_mockOpenType == null)
                throw new InvalidOperationException("Unable to find Type Moq.Mock<T> in assembly " + moq.Location);
        }

        public object CreateMock(Type type)
        {
            var closedType = _mockOpenType.MakeGenericType(type);
            var objectProperty = closedType.GetProperty("Object", type);
            var instance = Activator.CreateInstance(closedType);
            return objectProperty.GetValue(instance, null);
        }

        public object CreateMockThatCallsBase(Type type, object[] args)
        {
            var closedType = _mockOpenType.MakeGenericType(type);
            var callBaseProperty = closedType.GetProperty("CallBase", typeof (bool));
            var objectProperty = closedType.GetProperty("Object", type);
            var constructor = closedType.GetConstructor(new[] {typeof (object[])});
            var instance = constructor.Invoke(new[] {args});
            callBaseProperty.SetValue(instance, true, null);
            return objectProperty.GetValue(instance, null);
        }
    }
}