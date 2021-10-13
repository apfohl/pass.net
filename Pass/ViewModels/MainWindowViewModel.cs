using Pass.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels
{
    [View(typeof(MainWindow))]
    public sealed class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";
    }
}