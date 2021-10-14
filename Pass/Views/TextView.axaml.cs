using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Pass.Views
{
    public sealed class TextView : UserControl
    {
        public TextView() => AvaloniaXamlLoader.Load(this);
    }
}