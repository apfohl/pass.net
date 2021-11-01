using System.Windows.Input;
using Pass.Components.Binding;
using Pass.Components.Commands;
using Pass.Components.MessageBus;
using Pass.Components.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels
{
    public record Unlocked(string Password);

    [View(typeof(UnlockView))]
    public sealed class UnlockViewModel : Bindable
    {
        private readonly MessageBus messageBus;

        public string Password { get; set; }

        public ICommand Unlock => new RelayCommand(
            () => messageBus.Publish(new Unlocked(Password)),
            () => true);

        public UnlockViewModel(MessageBus messageBus) => this.messageBus = messageBus;
    }
}