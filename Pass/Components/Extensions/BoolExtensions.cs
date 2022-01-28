using System;
using System.Threading.Tasks;
using MonadicBits;

namespace Pass.Components.Extensions;

using static Functional;

public interface ITrue
{
    void OnFalse(Action action);
    Task OnFalse(Func<Task> func);
}

public interface IFalse
{
    void OnTrue(Action action);
    Task OnTrue(Func<Task> func);
}

public static class BoolExtensions
{
    private sealed record FluentBool(bool IsTrue) : ITrue, IFalse
    {
        public void OnFalse(Action action)
        {
            if (IsTrue)
            {
                throw new Exception($"Not allowed on {nameof(ITrue)}!");
            }

            action();
        }

        public Task OnFalse(Func<Task> func)
        {
            if (IsTrue)
            {
                throw new Exception($"Not allowed on {nameof(ITrue)}!");
            }

            return func();
        }

        public void OnTrue(Action action)
        {
            if (!IsTrue)
            {
                throw new Exception($"Not allowed on {nameof(ITrue)}!");
            }

            action();
        }

        public Task OnTrue(Func<Task> func)
        {
            if (!IsTrue)
            {
                throw new Exception($"Not allowed on {nameof(ITrue)}!");
            }

            return func();
        }
    }

    public static ITrue OnTrue(this bool value, Action action)
    {
        if (value)
        {
            action();
        }

        return new FluentBool(true);
    }

    public static IFalse OnFalse(this bool value, Action action)
    {
        if (!value)
        {
            action();
        }

        return new FluentBool(false);
    }

    public static async Task<ITrue> OnTrue(this bool value, Func<Task> func)
    {
        if (value)
        {
            await func();
        }

        return new FluentBool(true);
    }

    public static async Task<IFalse> OnFalse(this bool value, Func<Task> func)
    {
        if (!value)
        {
            await func();
        }

        return new FluentBool(false);
    }

    public static async Task<ITrue> OnTrue(this Task<bool> value, Func<Task> func) =>
        await (await value).OnTrue(func);

    public static async Task<IFalse> OnFalse(this Task<bool> value, Func<Task> func) =>
        await (await value).OnFalse(func);

    public static Maybe<T> OnTrue<T>(this bool value, Func<T> action) => value ? action() : Nothing;

    public static Maybe<T> OnFalse<T>(this bool value, Func<T> action) => !value ? action() : Nothing;
}