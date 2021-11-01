#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using MonadicBits;

namespace Pass.Components.Extensions
{
    using static Functional;

    public static class FunctionalExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this Maybe<T> @this) =>
            @this.Match(Elevate, Enumerable.Empty<T>);

        private static IEnumerable<T> Elevate<T>(this T @this)
        {
            yield return @this;
        }

        public static Maybe<T> SingleOrNothing<T>(this IEnumerable<T> @this, Func<T, bool> predicate) =>
            @this.Where(predicate)
                .Select(i => i.Just())
                .DefaultIfEmpty(Nothing)
                .Single();

        public static Maybe<T> SingleOrNothing<T>(this IEnumerable<T> @this) =>
            @this.SingleOrNothing(_ => true);

        public static Maybe<T> ToMaybe<T>(this T? @this) => @this == null ? Nothing : @this;
    }   
}