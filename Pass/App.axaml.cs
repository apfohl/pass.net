using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Bridgefield.PersistentBits;
using Pass.Components.FileSystem;
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
                var fileSystem = OS.FileSystem();
                var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var passwordDirectory = fileSystem
                    .OpenDirectory(Path.Combine(userProfilePath, ".password-store", "decrypted"))
                    .Match(d => d, () => throw new ArgumentException("Pass directory is missing!"));

                var mainWindow = new MainView();
                desktop.MainWindow = mainWindow;
                mainWindow.DataContext = new MainViewModel(new PasswordRepository(passwordDirectory));
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}