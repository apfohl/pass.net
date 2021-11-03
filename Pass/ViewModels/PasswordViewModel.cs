using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Pass.Components.Binding;
using Pass.Components.Commands;
using Pass.Components.ViewMapping;
using Pass.Models;
using Pass.Views;

namespace Pass.ViewModels
{
    [View(typeof(PasswordView))]
    public sealed class PasswordViewModel : Bindable
    {
        private readonly string password;
        private bool isHidden = true;
        private IDictionary<string, string> metadata;

        public string Name { get; }

        public string Password => isHidden ? "●●●●●●" : password;

        public Geometry ButtonIcon => isHidden
            ? Resource<Geometry>("EyeHideRegular")
            : Resource<Geometry>("EyeShowRegular");

        public ICommand ToggleHide => new RelayCommand(() =>
        {
            isHidden = !isHidden;
            OnPropertyChanged(nameof(Password));
            OnPropertyChanged(nameof(ButtonIcon));
            return Task.CompletedTask;
        }, () => true);

        public ICommand CopyToClipboard =>
            new RelayCommand(() => Application.Current.Clipboard.SetTextAsync(password), () => true);

        public PasswordViewModel(Password password)
        {
            Name = password.Name;
            this.password = password.Value;
            metadata = password.Metadata;
        }

        private static T Resource<T>(string key) where T : class =>
            Application.Current.FindResource(key) as T;
    }
}