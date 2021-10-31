using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Pass.Views
{
    public sealed class ContentWithSidebarView : UserControl
    {
        public ContentWithSidebarView() => AvaloniaXamlLoader.Load(this);
    }
}