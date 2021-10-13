using Pass.Components.Binding;
using Pass.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels
{
    [View(typeof(MainWindow))]
    public sealed class MainWindowViewModel : Bindable
    {
        public static string Greeting => "Welcome to Avalonia!";
    }
}