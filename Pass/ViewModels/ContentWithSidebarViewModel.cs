using System;
using System.Collections.Generic;
using bridgefield.FoundationalBits;
using Pass.Components.Binding;
using Pass.Components.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels;

[View(typeof(ContentWithSidebarView))]
public sealed class ContentWithSidebarViewModel : Bindable, IDisposable, IHandle<SelectedPasswordChanged>,
    IHandle<PasswordLoading>
{
    private readonly List<IDisposable> subscriptions = new();
    private readonly ReactiveProperty<Bindable> content;

    public Bindable Content
    {
        get => content.Value;
        set => content.Value = value;
    }

    public Bindable Sidebar { get; }

    public ContentWithSidebarViewModel(Bindable initialContent, Bindable sidebar, IMessageBus messageBus)
    {
        Sidebar = sidebar;

        content = new ReactiveProperty<Bindable>(initialContent);

        subscriptions.Add(content.Changed.Subscribe(_ => OnPropertyChanged(nameof(Content))));

        messageBus.Subscribe(this);
    }

    public void Handle(SelectedPasswordChanged message) => Content = new PasswordViewModel(message.Password);

    public void Handle(PasswordLoading message) => Content = new LoadingViewModel();

    public void Dispose() => subscriptions.ForEach(d => d.Dispose());
}