using Pass.Components.Binding;
using Pass.Components.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels
{
    [View(typeof(PasswordView))]
    public sealed class PasswordViewModel : Bindable, IContent
    {
        public string Name { get; }
        public string Password { get; }

        public PasswordViewModel(string name, string password)
        {
            Name = name;
            Password = password;
        }
    }
}