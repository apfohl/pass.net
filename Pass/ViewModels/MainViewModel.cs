using Bridgefield.PersistentBits.FileSystem;
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
        public Bindable Content { get; }

        public MainViewModel(IDirectory passwordDirectory, KeyRepository keyRepository, MessageBus messageBus) =>
            Content = new ContentWithSidebarViewModel(
                new TextViewModel("No password selected!"),
                new PasswordListViewModel(new PasswordRepository(passwordDirectory), messageBus, keyRepository),
                messageBus);
    }
}