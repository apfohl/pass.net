using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Pass.Components.Binding
{
    public sealed class ReactiveProperty<T> : IObservable<T>
    {
        private readonly BehaviorSubject<T> subject;

        public ReactiveProperty() : this(default)
        {
        }

        public ReactiveProperty(T initialValue) =>
            subject = new BehaviorSubject<T>(initialValue);

        public T Value
        {
            get => subject.Value;
            set => subject.OnNext(value);
        }

        public IObservable<Unit> Changed => subject.DistinctUntilChanged().Select(_ => Unit.Default);

        public IDisposable Subscribe(IObserver<T> observer) =>
            subject.DistinctUntilChanged().Subscribe(observer);
    }
}