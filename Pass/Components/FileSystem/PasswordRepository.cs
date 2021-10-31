using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bridgefield.PersistentBits.FileSystem;
using MonadicBits;
using Pass.Components.Extensions;

namespace Pass.Components.FileSystem
{
    public sealed class PasswordRepository
    {
        private readonly IDirectory rootDirectory;

        public PasswordRepository(IDirectory rootDirectory) =>
            this.rootDirectory = rootDirectory;

        public Maybe<IFile> Fingerprint() =>
            rootDirectory.Files.SingleOrNothing(file => file.Name == ".gpg-id");

        public Maybe<IEncryptedFile> Find(string name) =>
            FindAll().SingleOrNothing(file => file.Name == name);

        public IEnumerable<IEncryptedFile> FindAll() =>
            Directory
                .EnumerateFiles(rootDirectory.Path)
                .Where(IsPasswordFile)
                .Select(path => new EncryptedFile(path));

        private static bool IsPasswordFile(string fileName) =>
            !fileName.StartsWith('.') && fileName.EndsWith(".gpg");
    }
}