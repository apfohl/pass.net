using System;
using System.Reactive.Linq;
using Pass.Components.Binding;
using Pass.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels
{
    [View(typeof(MainView))]
    public sealed class MainViewModel : Bindable, IDisposable
    {
        private readonly ReactiveProperty<string> greeting = new("Welcome to Avalonia!");
        private readonly IDisposable subscription;

        public string Greeting
        {
            get => greeting.Value;
            set => greeting.Value = value;
        }

        public MainViewModel() =>
            subscription = greeting.Changed.Select(_ => nameof(Greeting)).Subscribe(OnPropertyChanged);

        public void Dispose() => subscription.Dispose();
    }
}