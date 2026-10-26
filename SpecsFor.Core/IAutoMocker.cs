using System;
using Moq;

namespace SpecsFor.Core
{
    public interface IAutoMocker : IDisposable
    {
        TSut CreateSUT<TSut>() where TSut : class;

        Mock<T> GetMockFor<T>() where T : class;

        void ConfigureContainer();
    }
}