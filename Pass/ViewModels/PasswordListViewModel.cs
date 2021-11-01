using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Pass.Components.Binding;
using Pass.Components.Extensions;
using Pass.Components.FileSystem;
using Pass.Components.MessageBus;
using Pass.Components.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels
{
    public record SelectedPasswordChanged(IContent ViewModel);

    [View(typeof(PasswordListView))]
    public sealed class PasswordListViewModel : Bindable, ISidebar, IDisposable
    {
        private readonly PasswordRepository passwordRepository;
        private readonly ReactiveProperty<string> searchString = new(string.Empty);
        private readonly ReactiveProperty<PasswordListItemViewModel> selectedPassword = new();
        private readonly List<IDisposable> subscriptions = new();

        public IEnumerable<PasswordListItemViewModel> Passwords =>
            passwordRepository
                .FindAll()
                .OrderBy(file => file.Name)
                .Select(file => file.Name.RemoveFromEnd(".gpg"))
                .Where(file => ContainsString(file, SearchString))
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

        public PasswordListViewModel(PasswordRepository passwordRepository, MessageBus messageBus)
        {
            this.passwordRepository = passwordRepository;

            subscriptions.Add(searchString.Changed.Subscribe(_ => OnPropertyChanged(nameof(Passwords))));
            subscriptions.Add(selectedPassword.Skip(1).Where(p => p != null).SelectMany(p =>
                    messageBus
                        .Publish(new SelectedPasswordChanged(new PasswordViewModel(p.Name))))
                .Subscribe());
        }

        public void Dispose() => subscriptions.ForEach(s => s.Dispose());

        private static bool ContainsString(string @this, string searchString) =>
            string.IsNullOrEmpty(searchString) || @this.Contains(searchString);
    }
}