using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Pass.Components.Binding;
using Pass.Components.Commands;
using Pass.Components.Dialog;
using Pass.Components.Extensions;
using Pass.Components.FileSystem;
using Pass.Components.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels
{
    [View(typeof(MainView))]
    public sealed class MainViewModel : Bindable, IDisposable
    {
        private readonly IDialogPresenter dialogPresenter;
        private readonly PasswordRepository passwordRepository;
        private readonly ReactiveProperty<string> greeting = new("Welcome to Avalonia!");
        private readonly ReactiveProperty<string> searchString = new(string.Empty);
        private readonly ReactiveProperty<PasswordViewModel> selectedPassword = new();
        private readonly List<IDisposable> subscriptions = new();

        public string Greeting
        {
            get => greeting.Value;
            set => greeting.Value = value;
        }

        public ICommand OpenDialog =>
            new RelayCommand(
                () => dialogPresenter.Show(new DefaultDialogViewModel(new TextViewModel("This is inside the dialog."))),
                () => true);

        public IEnumerable<PasswordViewModel> Passwords =>
            passwordRepository
                .FindAll()
                .OrderBy(file => file.Name)
                .Select(file => file.Name.RemoveFromEnd(".gpg"))
                .Where(file => ContainsString(file, SearchString))
                .Select(name => new PasswordViewModel(name))
                .ToList();

        public PasswordViewModel SelectedPassword
        {
            get => selectedPassword.Value;
            set => selectedPassword.Value = value;
        }

        public string SearchString
        {
            get => searchString.Value;
            set => searchString.Value = value;
        }

        public string PasswordName =>
            SelectedPassword != default(PasswordViewModel) ? SelectedPassword.Name : "No password selected!";

        public MainViewModel(IDialogPresenter dialogPresenter, PasswordRepository passwordRepository)
        {
            this.dialogPresenter = dialogPresenter;
            this.passwordRepository = passwordRepository;

            subscriptions.Add(greeting.Changed.Select(_ => nameof(Greeting)).Subscribe(OnPropertyChanged));
            subscriptions.Add(searchString.Changed.Subscribe(_ => OnPropertyChanged(nameof(Passwords))));
            subscriptions.Add(selectedPassword.Changed.Skip(1).Subscribe(_ => OnPropertyChanged(nameof(PasswordName))));
        }

        public void Dispose() => subscriptions.ForEach(s => s.Dispose());

        private static bool ContainsString(string @this, string searchString) =>
            string.IsNullOrEmpty(searchString) || @this.Contains(searchString);
    }
}