using Pass.Components.Binding;

namespace Pass.ViewModels
{
    public sealed class PasswordViewModel : Bindable
    {
        public string Name { get; }

        public PasswordViewModel(string name) => Name = name;
    }
}