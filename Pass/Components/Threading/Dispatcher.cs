using System;
using System.Threading.Tasks;
using MonadicBits;

namespace Pass.Components.Threading
{
    public static class Dispatcher
    {
        public static void Dispatch(Action action) =>
            Avalonia.Threading.Dispatcher.UIThread
                .JustWhen(d => !d.CheckAccess())
                .Match(d => d.Post(action), action);

        public static Task Dispatch(Func<Task> func)
        {
            var taskCompletionSource = new TaskCompletionSource();

            Dispatch((Action) (() => func().ContinueWith(
                task =>
                {
                    if (task.IsFaulted)
                    {
                        taskCompletionSource.SetException(task.Exception!);
                    }
                    else
                    {
                        taskCompletionSource.SetResult();
                    }
                })));

            return taskCompletionSource.Task;
        }

        public static Task<T> Dispatch<T>(Func<Task<T>> func)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();
            Dispatch((Action) (() => func().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    taskCompletionSource.SetException(task.Exception!);
                }
                else
                {
                    taskCompletionSource.SetResult(task.Result);
                }
            })));

            return taskCompletionSource.Task;
        }
    }
}