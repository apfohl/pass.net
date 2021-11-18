using System;
using System.Collections.Generic;
using Bridgefield.PersistentBits.FileSystem;
using JetBrains.Annotations;
using MonadicBits;
using Pass.Components.Binding;
using Pass.Components.Encryption;
using Pass.Components.FileSystem;
using Pass.Components.MessageBus;
using Pass.Components.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels;

using static Functional;

[View(typeof(MainView))]
public sealed class MainViewModel : Bindable, IDisposable
{
    private readonly IDirectory passwordDirectory;
    private readonly KeyRepository keyRepository;
    private readonly MessageBus messageBus;
    private readonly Stack<Bindable> contentStack = new();

    public Bindable Content => contentStack.Peek();

    public MainViewModel(IDirectory passwordDirectory, KeyRepository keyRepository, MessageBus messageBus)
    {
        this.passwordDirectory = passwordDirectory;
        this.keyRepository = keyRepository;
        this.messageBus = messageBus;

        contentStack.Push(new UnlockViewModel(messageBus));

        messageBus.Subscribe(this);
    }

    [UsedImplicitly]
    public void Handle(Unlocked message)
    {
        keyRepository.Password = message.Password;

        contentStack.Push(new ContentWithSidebarViewModel(
            new TextViewModel("No password selected!"),
            new PasswordListViewModel(new PasswordRepository(passwordDirectory), messageBus, keyRepository),
            messageBus));

        OnPropertyChanged(nameof(Content));
    }

    [UsedImplicitly]
    public void Handle(Locked message)
    {
        keyRepository.Password = Nothing;
        contentStack.Pop();
        OnPropertyChanged(nameof(Content));
    }

    public void Dispose() => messageBus.Unsubscribe(this);
}