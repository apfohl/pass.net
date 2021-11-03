using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using MonadicBits;
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
    public record SelectedPasswordChanged(Bindable ViewModel);

    [View(typeof(PasswordListView))]
    public sealed class PasswordListViewModel : Bindable, IDisposable
    {
        private readonly PasswordRepository passwordRepository;
        private readonly MessageBus messageBus;
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

        public ICommand Lock => new RelayCommand(() => messageBus.Publish(new Locked()), () => true);

        public PasswordListViewModel(PasswordRepository passwordRepository, MessageBus messageBus,
            KeyRepository keyRepository)
        {
            this.passwordRepository = passwordRepository;
            this.messageBus = messageBus;
            this.keyRepository = keyRepository;

            subscriptions.Add(searchString.Changed.Subscribe(_ => OnPropertyChanged(nameof(Passwords))));
            subscriptions.Add(selectedPassword.Skip(1).Where(p => p != null)
                .SelectMany(async p => (await DecryptedPassword(p.Name))
                    .Match(
                        async password =>
                            await messageBus.Publish(new SelectedPasswordChanged(new PasswordViewModel(password))),
                        () => Task.CompletedTask))
                .Subscribe());
        }

        public void Dispose() => subscriptions.ForEach(s => s.Dispose());

        private static bool ContainsString(string @this, string searchString) =>
            string.IsNullOrEmpty(searchString) || @this.Contains(searchString);

        private Task<Maybe<Password>> DecryptedPassword(string name)
        {
            var stream = from file in passwordRepository.Find($"{name}.gpg")
                from keyStream in keyRepository.PrivateKey.Bind(keyFile => keyFile.OpenRead())
                from password in keyRepository.Password
                from decryptedStream in DecryptedStream(file, keyStream, password)
                select decryptedStream;

            return stream.BindAsync(async s =>
            {
                await using (s)
                {
                    return await Password(name, s);
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

        private static async Task<Maybe<Password>> Password(string name, Stream stream)
        {
            using var reader = new StreamReader(stream);

            return await (await ReadPassword(reader))
                .MapAsync(async password =>
                {
                    var metadata = await ReadLines(reader).AggregateAsync(new Dictionary<string, string>(),
                        (dictionary, line) =>
                        {
                            ParseLine(line).Match(tuple => dictionary.Add(tuple.key, tuple.value), () => { });
                            return dictionary;
                        });

                    return new Password(name, password, metadata);
                });
        }

        private static Maybe<(string key, string value)> ParseLine(string line) =>
            new Regex("^(.+):(.+)$")
                .MatchInput(line)
                .Map(match => match.Groups)
                .Map(groups => (groups[1].Value, groups[2].Value));

        private static async Task<Maybe<string>> ReadPassword(TextReader reader) =>
            (await reader.ReadLineAsync()).ToMaybe();

        private static async IAsyncEnumerable<string> ReadLines(TextReader reader)
        {
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                yield return line;
            }
        }
    }
}