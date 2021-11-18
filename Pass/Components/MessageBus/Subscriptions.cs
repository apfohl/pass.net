using System;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Threading.Tasks;
using MonadicBits;
using Pass.Components.Async;

namespace Pass.Components.MessageBus;

using static Functional;

public sealed record SubscriberMethod(MethodInfo Method, Type ArgumentType)
{
    public Maybe<object> Invoke(object target, object argument) =>
        Method.Invoke(target, new[] { argument }) ?? Nothing;
}

public sealed record Handler(object Target, SubscriberMethod Method)
{
    public Task Post(object argument) =>
        Method.Invoke(Target, argument).Match(
            j =>
            {
                if (j is Task t)
                {
                    return t;
                }

                return Task.FromResult(j);
            },
            () => Task.CompletedTask);
}

public sealed record Subscription(WeakReference Subscriber, SubscriberMethod[] Methods)
{
    public bool CanHandle(Type argumentType) =>
        Method(argumentType).Match(_ => true, () => false);

    public Maybe<Handler> Handler(Type argumentType) =>
        Subscriber.JustWhen(s => s.IsAlive)
            .Bind(s => s.Target.JustNotNull())
            .Bind(t => Method(argumentType).Map(m => new Handler(t, m)));

    public static Subscription FromSubscriber(object subscriber)
    {
        var type = subscriber.GetType();
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(m => m.Name == "Handle" && m.GetParameters().Length == 1)
            .Select(m => new SubscriberMethod(m, m.GetParameters().First().ParameterType));
        return new Subscription(new WeakReference(subscriber), methods.ToArray());
    }

    private Maybe<SubscriberMethod> Method(Type argumentType) =>
        Methods.Where(h => h.ArgumentType.IsAssignableFrom(argumentType)).FirstOrNothing();
}

public sealed class MessageBus
{
    private sealed record State(Subscription[] Subscriptions);

    private readonly IAgent<SubscriptionCommand, Subscription[]> agent;

    public MessageBus() =>
        agent = IAgent<SubscriptionCommand, Subscription[]>.Start(
            new State(Array.Empty<Subscription>()),
            (state, command) => command.Execute(state));

    public void Subscribe(object subscriber) =>
        agent.Tell(new AddSubscription(Subscription.FromSubscriber(subscriber)));

    public void Unsubscribe(object subscriber) =>
        agent.Tell(new UnsubscribeTarget(subscriber));

    public async Task<Unit> Publish(object argument)
    {
        await Task.WhenAll(
            (await agent.Tell(new SelectSubscriptions(argument.GetType())))
            .Select(s => s.Handler(argument.GetType())
                .Match(
                    h => h.Post(argument),
                    () => agent.Tell(new RemoveSubscription(s))))
            .ToArray()
        );
            
        return Unit.Default;
    }

    private abstract record SubscriptionCommand
    {
        public abstract Task<(State, Subscription[])> Execute(State state);
    }

    private sealed record AddSubscription(Subscription NewSubscription) : SubscriptionCommand
    {
        public override Task<(State, Subscription[])> Execute(State state) =>
            AsTask(Unpack(state with
            {
                Subscriptions = state.Subscriptions.Concat(new[] { NewSubscription }).ToArray()
            }));
    }

    private sealed record UnsubscribeTarget(object Target) : SubscriptionCommand
    {
        public override Task<(State, Subscription[])> Execute(State state) =>
            AsTask(Unpack(state with
            {
                Subscriptions = state.Subscriptions.Except(
                    state.Subscriptions
                        .Where(s => s.Subscriber.Target == Target)).ToArray()
            }));
    }

    private sealed record RemoveSubscription(Subscription Subscription) : SubscriptionCommand
    {
        public override Task<(State, Subscription[])> Execute(State state) =>
            AsTask(Unpack(state with
            {
                Subscriptions = state.Subscriptions.Except(new[] { Subscription }).ToArray()
            }));
    }

    private sealed record SelectSubscriptions(Type ArgumentType) : SubscriptionCommand
    {
        public override Task<(State, Subscription[])> Execute(State state) =>
            AsTask((state,
                state.Subscriptions.Where(s => s.CanHandle(ArgumentType)).ToArray()));
    }

    private static (State State, Subscription[] Results) Unpack(State state) =>
        (state, state.Subscriptions);

    private static Task<(State State, Subscription[] Results)>
        AsTask((State State, Subscription[] Results) tuple) =>
        Task.FromResult(tuple);
}