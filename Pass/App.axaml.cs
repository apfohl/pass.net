using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Pass.Components.Dialog;
using Pass.ViewModels;
using Pass.Views;

namespace Pass
{
    public sealed class App : Application
    {
        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = new MainView();
                desktop.MainWindow = mainWindow;
                mainWindow.DataContext = new MainViewModel(new DefaultDialogPresenter(mainWindow));
                
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}