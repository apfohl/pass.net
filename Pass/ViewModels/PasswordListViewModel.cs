using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Pass.Components.Binding;
using Pass.Components.Extensions;
using Pass.Components.FileSystem;
using Pass.Components.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels
{
    [View(typeof(PasswordListView))]
    public sealed class PasswordListViewModel : Bindable, IDisposable
    {
        private readonly PasswordRepository passwordRepository;
        private readonly ReactiveProperty<string> searchString = new(string.Empty);
        private readonly ReactiveProperty<PasswordViewModel> selectedPassword = new();
        private readonly List<IDisposable> subscriptions = new();

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

        public PasswordListViewModel(PasswordRepository passwordRepository)
        {
            this.passwordRepository = passwordRepository;

            subscriptions.Add(searchString.Changed.Subscribe(_ => OnPropertyChanged(nameof(Passwords))));
            subscriptions.Add(selectedPassword.Changed.Skip(1).Subscribe(_ => { }));
        }

        public void Dispose() => subscriptions.ForEach(s => s.Dispose());

        private static bool ContainsString(string @this, string searchString) =>
            string.IsNullOrEmpty(searchString) || @this.Contains(searchString);
    }
}