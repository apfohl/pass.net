using Pass.Components.Binding;

namespace Pass.ViewModels
{
    public sealed class PasswordListItemViewModel : Bindable
    {
        public string Name { get; }

        public PasswordListItemViewModel(string name) => Name = name;
    }
}