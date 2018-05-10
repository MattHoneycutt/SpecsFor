using System;
using System.Linq;
using Moq;
using StructureMap.AutoMocking;

namespace SpecsFor
{
	public static class AutoMockerExtensions
	{
		public static Mock<TMock> GetMockFor<T, TMock>(this IAutoMocker<T> mocker) where TMock : class where T : class
		{
			return Mock.Get(mocker.Get<TMock>());
		}

		public static Mock<TMock>[] GetMockForEnumerableOf<T, TMock>(this IAutoMocker<T> mocker, int enumerableSize) where TMock : class where T : class
		{
			var existingMocks = mocker.Container.Model.InstancesOf<TMock>().ToArray();

			if (existingMocks.Length > 0)
			{
				if (existingMocks.Length != enumerableSize)
				{
					throw new InvalidOperationException("An IEnumerable for this type of mock has already been created with size " +
					                                    existingMocks.Length + ".");
				}

				return mocker.Container.GetAllInstances<TMock>().Select(Mock.Get).ToArray();
			}

			var mocks = mocker.CreateMockArrayFor<TMock>(enumerableSize);

			return mocks.Select(Mock.Get).ToArray();
		}
	}
}