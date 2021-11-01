using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MonadicBits;
using Pass.Components.Binding;
using Pass.Components.Encryption;
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
        private readonly KeyRepository keyRepository;
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

        public PasswordListViewModel(PasswordRepository passwordRepository, MessageBus messageBus,
            KeyRepository keyRepository)
        {
            this.passwordRepository = passwordRepository;
            this.keyRepository = keyRepository;

            subscriptions.Add(searchString.Changed.Subscribe(_ => OnPropertyChanged(nameof(Passwords))));
            subscriptions.Add(selectedPassword.Skip(1).Where(p => p != null).SelectMany(async p =>
                {
                    var password = await DecryptedPassword(p.Name);

                    return await messageBus.Publish(new SelectedPasswordChanged(new PasswordViewModel(p.Name,
                        password.Match(pass => pass, () => "Not found!"))));
                })
                .Subscribe());
        }

        public void Dispose() => subscriptions.ForEach(s => s.Dispose());

        private static bool ContainsString(string @this, string searchString) =>
            string.IsNullOrEmpty(searchString) || @this.Contains(searchString);

        private Task<Maybe<string>> DecryptedPassword(string name)
        {
            var stream = from file in passwordRepository.Find($"{name}.gpg")
                from keyStream in keyRepository.PrivateKey.Bind(keyFile => keyFile.OpenRead())
                from decryptedStream in DecryptedStream(file, keyStream, "Test")
                select decryptedStream;

            return stream.BindAsync(async s =>
            {
                await using (s)
                {
                    using var streamReader = new StreamReader(s);
                    return (await streamReader.ReadLineAsync()).ToMaybe();
                }
            });
        }

        private static Maybe<Stream> DecryptedStream(IEncryptedFile file, Stream keyStream, string password)
        {
            using (keyStream)
            {
                return file.OpenRead(keyStream, password);
            }
        }
    }
}