using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Pass.Views
{
    public sealed class DialogView : Window
    {
        public DialogView()
        {
            AvaloniaXamlLoader.Load(this);
            AttachDeveloperTools();
        }

        [Conditional("DEBUG")]
        private void AttachDeveloperTools() => this.AttachDevTools();
    }
}