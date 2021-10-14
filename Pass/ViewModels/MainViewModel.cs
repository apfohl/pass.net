using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Pass.Components.Binding;
using Pass.Components.Commands;
using Pass.Components.Dialog;
using Pass.Components.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels
{
    [View(typeof(MainView))]
    public sealed class MainViewModel : Bindable, IDisposable
    {
        private readonly IDialogPresenter dialogPresenter;
        private readonly ReactiveProperty<string> greeting = new("Welcome to Avalonia!");
        private readonly IDisposable subscription;

        public string Greeting
        {
            get => greeting.Value;
            set => greeting.Value = value;
        }

        public ICommand OpenDialog =>
            new RelayCommand(
                () => dialogPresenter.Show(new DefaultDialogViewModel(new TextViewModel("This is inside the dialog."))),
                () => true);

        public MainViewModel(IDialogPresenter dialogPresenter)
        {
            this.dialogPresenter = dialogPresenter;
            subscription = greeting.Changed.Select(_ => nameof(Greeting)).Subscribe(OnPropertyChanged);
        }

        public void Dispose() => subscription.Dispose();
    }
}