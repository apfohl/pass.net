using System;
using System.Threading.Tasks;

namespace Pass.Components.Async
{
    public interface IAgent<TCommand, TReply>
    {
        Task<TReply> Tell(TCommand command);

        static IAgent<TCommand, TReply> Start<TState>(
            TState initialState,
            Func<TState, TCommand, Task<(TState newState, TReply reply)>> update)
            => new StatefulAgent<TState, TCommand, TReply>(initialState, update);
    }
}