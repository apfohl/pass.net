using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Pass.Components.Binding;
using Pass.Components.Commands;
using Pass.Components.ViewMapping;
using Pass.Models;
using Pass.Views;

namespace Pass.ViewModels;

public record Meta(string Key, string Value);

[View(typeof(PasswordView))]
public sealed class PasswordViewModel : Bindable
{
    private readonly IDictionary<string, string> metadata;

    public string Name { get; }
    public string Password { get; }

    public IEnumerable<Meta> Metadata =>
        metadata.Select(pair => new Meta(pair.Key, pair.Value)).ToList();

    public ICommand CopyToClipboard =>
        new RelayCommand(() => Application.Current.Clipboard.SetTextAsync(Password), () => true);

    public PasswordViewModel(Password password)
    {
        Name = password.Name;
        Password = password.Value;
        metadata = password.Metadata;
    }
}