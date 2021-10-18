using Bridgefield.PersistentBits.FileSystem;
using Pass.Components.Binding;
using Pass.Components.Extensions;

namespace Pass.ViewModels
{
    public sealed class PasswordViewModel : Bindable
    {
        public string Name { get; }

        public PasswordViewModel(IFileSystemEntry file) => Name = file.Name.RemoveFromEnd(".gpg");
    }
}