#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using MonadicBits;

namespace Pass.Components.Extensions;

using static Functional;

public static class FunctionalExtensions
{
    public static Maybe<T> SingleOrNothing<T>(this IEnumerable<T> @this, Func<T, bool> predicate) =>
        @this.Where(predicate)
            .Select(i => i.Just())
            .DefaultIfEmpty(Nothing)
            .Single();

    public static Maybe<T> SingleOrNothing<T>(this IEnumerable<T> @this) =>
        @this.SingleOrNothing(_ => true);

    public static Maybe<T> ToMaybe<T>(this T? @this) => @this == null ? Nothing : @this;
}