using System;
using System.Threading.Tasks;
using MonadicBits;

namespace Pass.Components.Extensions;

using static Functional;

public static class BoolExtensions
{
    public static bool OnTrue(this bool value, Action action)
    {
        if (value)
        {
            action();
        }

        return value;
    }

    public static bool OnFalse(this bool value, Action action)
    {
        if (!value)
        {
            action();
        }

        return value;
    }

    public static async Task<bool> OnTrue(this bool value, Func<Task> func)
    {
        if (value)
        {
            await func();
        }

        return value;
    }

    public static async Task<bool> OnFalse(this bool value, Func<Task> func)
    {
        if (!value)
        {
            await func();
        }

        return value;
    }

    public static async Task<bool> OnTrue(this Task<bool> value, Func<Task> func) =>
        await (await value).OnTrue(func);

    public static async Task<bool> OnFalse(this Task<bool> value, Func<Task> func) =>
        await (await value).OnFalse(func);

    public static Maybe<T> OnTrue<T>(this bool value, Func<T> action) => value ? action() : Nothing;

    public static Maybe<T> OnFalse<T>(this bool value, Func<T> action) => !value ? action() : Nothing;
}