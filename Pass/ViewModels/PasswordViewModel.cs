using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Pass.Components.Binding;
using Pass.Components.Commands;
using Pass.Components.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels
{
    [View(typeof(PasswordView))]
    public sealed class PasswordViewModel : Bindable
    {
        private readonly string password;
        private bool isHidden = true;

        public string Name { get; }

        public string Password => isHidden ? "******" : password;

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

        public PasswordViewModel(string name, string password)
        {
            Name = name;
            this.password = password;
        }

        private static T Resource<T>(string key) where T : class =>
            Application.Current.FindResource(key) as T;
    }
}