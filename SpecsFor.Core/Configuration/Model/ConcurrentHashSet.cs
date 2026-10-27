using System.Collections.Concurrent;

namespace SpecsFor.Core.Configuration.Model
{
	internal class ConcurrentHashSet<T>
	{
		private readonly ConcurrentDictionary<T, bool> _dictionary = new ConcurrentDictionary<T, bool>();

		public bool Add(T item)
		{
			return _dictionary.TryAdd(item, true);
		}

		public bool Contains(T item)
		{
			return _dictionary.ContainsKey(item);
		}
	}
}
