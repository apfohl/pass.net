using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Pass.Components.Async
{
    public sealed class StatefulAgent<TState, TCommand, TReply> : IAgent<TCommand, TReply>
    {
        private readonly ActionBlock<(TCommand command, TaskCompletionSource<TReply> task)> actions;

        public StatefulAgent(TState initialState,
            Func<TState, TCommand, Task<(TState newState, TReply reply)>> processor)
        {
            var state = initialState;
            actions = new ActionBlock<(TCommand command, TaskCompletionSource<TReply> task)>(
                async t =>
                {
                    try
                    {
                        var result = await processor(state, t.command);
                        state = result.newState;
                        t.task.SetResult(result.reply);
                    }
                    catch (Exception e)
                    {
                        t.task.SetException(e);
                    }
                });
        }

        public Task<TReply> Tell(TCommand command)
        {
            var completionSource = new TaskCompletionSource<TReply>(TaskCreationOptions.RunContinuationsAsynchronously);
            actions.Post((command, completionSource));
            return completionSource.Task;
        }
    }
}