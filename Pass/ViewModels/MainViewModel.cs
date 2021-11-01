using Bridgefield.PersistentBits.FileSystem;
using JetBrains.Annotations;
using Pass.Components.Binding;
using Pass.Components.Encryption;
using Pass.Components.FileSystem;
using Pass.Components.MessageBus;
using Pass.Components.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels
{
    [View(typeof(MainView))]
    public sealed class MainViewModel : Bindable
    {
        private readonly IDirectory passwordDirectory;
        private KeyRepository keyRepository;
        private readonly MessageBus messageBus;
        public Bindable Content { get; private set; }

        public MainViewModel(IDirectory passwordDirectory, KeyRepository keyRepository, MessageBus messageBus)
        {
            this.passwordDirectory = passwordDirectory;
            this.keyRepository = keyRepository;
            this.messageBus = messageBus;

            Content = new UnlockViewModel(messageBus);

            messageBus.Subscribe(this);
        }

        [UsedImplicitly]
        public void Handle(Unlocked message)
        {
            keyRepository = keyRepository with { Password = message.Password };
            
            Content = new ContentWithSidebarViewModel(
                new TextViewModel("No password selected!"),
                new PasswordListViewModel(new PasswordRepository(passwordDirectory), messageBus, keyRepository),
                messageBus);

            OnPropertyChanged(nameof(Content));
        }
    }
}