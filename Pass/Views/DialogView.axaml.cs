using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Pass.Views
{
    public class DialogView : Window
    {
        public DialogView()
        {
            AvaloniaXamlLoader.Load(this);
            this.AttachDevTools();
        }

        [Conditional("DEBUG")]
        private void AttachDeveloperTools() => this.AttachDevTools();
    }
}