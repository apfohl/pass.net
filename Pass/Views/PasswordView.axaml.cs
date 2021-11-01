using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Pass.Views
{
    public class PasswordView : UserControl
    {
        public PasswordView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}