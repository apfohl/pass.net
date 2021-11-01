using System;
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
    public sealed class UnlockViewModel : Bindable, IDisposable
    {
        private readonly ReactiveProperty<string> password = new(string.Empty);
        private readonly RelayCommand unlock;

        public string Password
        {
            get => password.Value;
            set => password.Value = value;
        }

        public ICommand Unlock => unlock;

        public UnlockViewModel(MessageBus messageBus) =>
            unlock = new RelayCommand(
                () => messageBus.Publish(new Unlocked(Password)),
                () => !string.IsNullOrEmpty(Password),
                password.Changed);

        public void Dispose() => unlock.Dispose();
    }
}