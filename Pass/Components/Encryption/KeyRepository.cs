using Bridgefield.PersistentBits.FileSystem;
using MonadicBits;

namespace Pass.Components.Encryption
{
    public sealed class KeyRepository
    {
        public Maybe<IFile> PrivateKey { get; init; }
        public Maybe<IFile> PublicKey { get; init; }
        public Maybe<string> Password { get; set; }
    }
}