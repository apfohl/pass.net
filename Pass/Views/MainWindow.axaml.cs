using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Pass.Views
{
    public sealed class MainWindow : Window
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            
            AttachDeveloperTools();
        }

        [Conditional("DEBUG")]
        private void AttachDeveloperTools() => this.AttachDevTools();
    }
}