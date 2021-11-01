using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using JetBrains.Annotations;
using Pass.Components.Binding;
using Pass.Components.MessageBus;
using Pass.Components.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels
{
    public interface ISidebar
    {
    }

    public interface IContent
    {
    }

    [View(typeof(ContentWithSidebarView))]
    public sealed class ContentWithSidebarViewModel : Bindable, IDisposable
    {
        private readonly List<IDisposable> subscriptions = new();
        private readonly ReactiveProperty<IContent> content;

        public IContent Content
        {
            get => content.Value;
            set => content.Value = value;
        }

        public ISidebar Sidebar { get; }

        public ContentWithSidebarViewModel(IContent initialContent, ISidebar sidebar, MessageBus messageBus)
        {
            Sidebar = sidebar;

            content = new ReactiveProperty<IContent>(initialContent);

            subscriptions.Add(content.Changed.Subscribe(_ => OnPropertyChanged(nameof(Content))));

            messageBus.Subscribe(this);
        }

        [UsedImplicitly]
        public void Handle(SelectedPasswordChanged message) => Content = message.ViewModel;

        public void Dispose() => subscriptions.ForEach(d => d.Dispose());
    }
}