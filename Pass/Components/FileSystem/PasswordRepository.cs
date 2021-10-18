using System.Collections.Generic;
using System.Linq;
using Bridgefield.PersistentBits.FileSystem;
using MonadicBits;
using Pass.Components.Extensions;

namespace Pass.Components.FileSystem
{
    public sealed class PasswordRepository
    {
        private readonly IDirectory rootDirectory;

        public PasswordRepository(IDirectory rootDirectory) => this.rootDirectory = rootDirectory;

        public Maybe<IFile> Find(string name) => FindAll().SingleOrNothing(file => file.Name == name);

        public IEnumerable<IFile> FindAll() => rootDirectory.Files.Where(IsPasswordFile);

        private static bool IsPasswordFile(IFile file) => !file.Name.StartsWith('.') && file.Name.EndsWith(".gpg");
    }
}