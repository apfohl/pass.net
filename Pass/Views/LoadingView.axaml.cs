using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Pass.Views
{
    public sealed class LoadingView : UserControl
    {
        public LoadingView() => AvaloniaXamlLoader.Load(this);
    }
}