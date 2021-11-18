using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Pass.Views;

public sealed class PasswordView : UserControl
{
    public PasswordView() => AvaloniaXamlLoader.Load(this);
}