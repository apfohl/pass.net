using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Pass.Components.Binding;
using Pass.Components.MessageBus;
using Pass.Components.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels
{
    [View(typeof(ContentWithSidebarView))]
    public sealed class ContentWithSidebarViewModel : Bindable, IDisposable
    {
        private readonly List<IDisposable> subscriptions = new();
        private readonly ReactiveProperty<Bindable> content;

        public Bindable Content
        {
            get => content.Value;
            set => content.Value = value;
        }

        public Bindable Sidebar { get; }

        public ContentWithSidebarViewModel(Bindable initialContent, Bindable sidebar, MessageBus messageBus)
        {
            Sidebar = sidebar;

            content = new ReactiveProperty<Bindable>(initialContent);

            subscriptions.Add(content.Changed.Subscribe(_ => OnPropertyChanged(nameof(Content))));

            messageBus.Subscribe(this);
        }

        [UsedImplicitly]
        public void Handle(SelectedPasswordChanged message) => Content = new PasswordViewModel(message.Password);

        [UsedImplicitly]
        public void Handle(PasswordLoading message) => Content = new LoadingViewModel();

        public void Dispose() => subscriptions.ForEach(d => d.Dispose());
    }
}