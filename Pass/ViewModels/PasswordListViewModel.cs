using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Pass.Components.Binding;
using Pass.Components.Commands;
using Pass.Components.Encryption;
using Pass.Components.Extensions;
using Pass.Components.FileSystem;
using Pass.Components.MessageBus;
using Pass.Components.ViewMapping;
using Pass.Models;
using Pass.Views;

namespace Pass.ViewModels
{
    public record SelectedPasswordChanged(Password Password);

    public record PasswordLoading;

    [View(typeof(PasswordListView))]
    public sealed class PasswordListViewModel : Bindable, IDisposable
    {
        private readonly PasswordRepository passwordRepository;
        private readonly MessageBus messageBus;
        private readonly ReactiveProperty<string> searchString = new(string.Empty);
        private readonly ReactiveProperty<PasswordListItemViewModel> selectedPassword = new();
        private readonly List<IDisposable> subscriptions = new();

        public IEnumerable<PasswordListItemViewModel> Passwords =>
            passwordRepository
                .FindAll()
                .OrderBy(file => file.Name)
                .Select(file => file.Name.RemoveFromEnd(".gpg"))
                .Where(name => name.ContainsString(SearchString))
                .Select(name => new PasswordListItemViewModel(name))
                .ToList();

        public PasswordListItemViewModel SelectedPassword
        {
            get => selectedPassword.Value;
            set => selectedPassword.Value = value;
        }

        public string SearchString
        {
            get => searchString.Value;
            set => searchString.Value = value;
        }

        public ICommand Lock => new RelayCommand(() => messageBus.Publish(new Locked()), () => true);

        public PasswordListViewModel(
            PasswordRepository passwordRepository,
            MessageBus messageBus,
            KeyRepository keyRepository)
        {
            this.passwordRepository = passwordRepository;
            this.messageBus = messageBus;

            subscriptions.Add(searchString.Changed.Subscribe(_ => OnPropertyChanged(nameof(Passwords))));
            subscriptions.Add(selectedPassword.Skip(1).Where(p => p != null)
                .SelectMany(async p =>
                {
                    await messageBus.Publish(new PasswordLoading());

                    return (await Decrypt.DecryptedPassword(passwordRepository, keyRepository, p.Name))
                        .Match(password => messageBus.Publish(
                                new SelectedPasswordChanged(password)),
                            () => Task.CompletedTask);
                })
                .Subscribe());
        }

        public void Dispose() => subscriptions.ForEach(s => s.Dispose());
    }
}