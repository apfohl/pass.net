using System;
using System.Windows.Input;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using JetBrains.Annotations;
using Pass.Components.Binding;
using Pass.Components.Commands;
using Pass.Components.MessageBus;
using Pass.Components.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels
{
    public sealed record Unlocked(string Password);

    public sealed record Locked;

    [View(typeof(UnlockView))]
    public sealed class UnlockViewModel : Bindable, IDisposable
    {
        private readonly MessageBus messageBus;
        private readonly ReactiveProperty<string> password = new(string.Empty);
        private readonly RelayCommand unlock;

        public string Password
        {
            get => password.Value;
            set => password.Value = value;
        }

        public ICommand Unlock => unlock;

        public int SpinnerSize => 100;

        public UnlockViewModel(MessageBus messageBus)
        {
            this.messageBus = messageBus;
            
            unlock = new RelayCommand(
                () => messageBus.Publish(new Unlocked(Password)),
                () => !string.IsNullOrEmpty(Password),
                password.Changed);
            
            messageBus.Subscribe(this);
        }

        public void Dispose()
        {
            unlock.Dispose();
            messageBus.Unsubscribe(this);
        }

        [UsedImplicitly]
        public void Handle(Locked message)
        {
            password.Value = string.Empty;
            OnPropertyChanged(nameof(Password));
        }
    }
}