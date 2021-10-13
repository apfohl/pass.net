using System.Collections.Generic;
using System.Linq;
using MonadicBits;

namespace Pass.Components
{
    using static Functional;

    public static class FunctionalExtensions
    {
        public static Maybe<T> SingleOrNothing<T>(this IEnumerable<T> source) =>
            source
                .Select(i => i.Just())
                .DefaultIfEmpty(Nothing)
                .Single();
    }
}