using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SpecsFor.Shouldly
{
    public static class EnumerableExtensions
    {
        public static void ShouldBeAscending<T>(this IEnumerable<T> values) where T : IComparable
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            var position = 0;
            var enumerator = values.GetEnumerator();

            if (!enumerator.MoveNext()) return;

            var old = enumerator.Current;

            while (enumerator.MoveNext())
            {
                position++;
                if (enumerator.Current.CompareTo(old) <= 0)
                    throw new AssertionException($"Found non-ascending values at position {position - 1}, {position}: {old}, {enumerator.Current}");
            }
        }

        public static void ShouldBeDescending<T>(this IEnumerable<T> values) where T : IComparable
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            var position = 0;
            var enumerator = values.GetEnumerator();

            if (!enumerator.MoveNext()) return;

            var old = enumerator.Current;

            while (enumerator.MoveNext())
            {
                position++;
                if (enumerator.Current.CompareTo(old) >= 0)
                    throw new AssertionException($"Found non-descending values at position {position - 1}, {position}: {old}, {enumerator.Current}");
            }
        }
    }
}