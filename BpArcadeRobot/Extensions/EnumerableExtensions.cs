using System.Collections.Generic;

namespace System.Linq
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<(T, int)> GlueSequence<T>(this IEnumerable<T> items, Func<T, int> func)
        {
            items = items.OrderBy(x => func(x));

            while (items.Any())
            {
                var first = items.First();
                var firstNumber = func(first);
                var taken = items.TakeWhile((x, i) => func(x) == func(first) + i);
                yield return (first, taken.Count());

                items = items.Skip(taken.Count());
            }
        }
    }
}
