using System;
using System.Threading.Tasks;
using MonadicBits;

namespace Pass.Components.Extensions;

using static Functional;

public readonly struct FluentTrue
{
    private readonly bool value;

    public FluentTrue(bool value) => this.value = value;

    public void OnFalse(Action action)
    {
        if (!value)
        {
            action();
        }
    }

    public Task OnFalse(Func<Task> func) =>
        !value ? func() : Task.CompletedTask;
}

public readonly struct FluentFalse
{
    private readonly bool value;

    public FluentFalse(bool value) => this.value = value;

    public void OnTrue(Action action)
    {
        if (value)
        {
            action();
        }
    }

    public Task OnTrue(Func<Task> func) =>
        value ? func() : Task.CompletedTask;
}

public static class BoolExtensions
{
    public static FluentTrue OnTrue(this bool value, Action action)
    {
        if (value)
        {
            action();
        }

        return new FluentTrue(value);
    }

    public static FluentFalse OnFalse(this bool value, Action action)
    {
        if (!value)
        {
            action();
        }

        return new FluentFalse();
    }

    public static async Task<FluentTrue> OnTrue(this bool value, Func<Task> func)
    {
        if (value)
        {
            await func();
        }

        return new FluentTrue(value);
    }

    public static async Task<FluentFalse> OnFalse(this bool value, Func<Task> func)
    {
        if (!value)
        {
            await func();
        }

        return new FluentFalse(value);
    }

    public static async Task<FluentTrue> OnTrue(this Task<bool> value, Func<Task> func) =>
        await (await value).OnTrue(func);

    public static async Task<FluentFalse> OnFalse(this Task<bool> value, Func<Task> func) =>
        await (await value).OnFalse(func);

    public static Maybe<T> OnTrue<T>(this bool value, Func<T> action) => value ? action() : Nothing;

    public static Maybe<T> OnFalse<T>(this bool value, Func<T> action) => !value ? action() : Nothing;
}