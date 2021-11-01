using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Bridgefield.PersistentBits;
using Bridgefield.PersistentBits.FileSystem;
using MonadicBits;
using Pass.Components.Encryption;
using Pass.Components.Extensions;
using Pass.Components.MessageBus;
using Pass.ViewModels;
using Pass.Views;

namespace Pass
{
    using static Functional;
    
    public sealed class App : Application
    {
        private static readonly string UserProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var fileSystem = OS.FileSystem();
                var passwordDirectory = fileSystem
                    .OpenDirectory(Path.Combine(UserProfilePath, ".password-store"))
                    .Match(d => d, () => throw new ArgumentException("Pass directory is missing!"));
                var keyRepository = KeyRepository(fileSystem);
                var messageBus = new MessageBus();

                var mainWindow = new MainView();
                desktop.MainWindow = mainWindow;
                mainWindow.DataContext = new MainViewModel(passwordDirectory, keyRepository, messageBus);
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static KeyRepository KeyRepository(IFileSystem fileSystem)
        {
            var directory = fileSystem.OpenDirectory(Path.Combine(UserProfilePath, "Documents"))
                .Match(d => d, () => throw new ArgumentException("Key directory is missing!"));

            return new KeyRepository(
                directory.Files.Where(file => file.Name == "private.asc").SingleOrNothing(),
                directory.Files.Where(file => file.Name == "public.asc").SingleOrNothing(),
                Nothing
            );
        }
    }
}