using Bridgefield.PersistentBits.FileSystem;
using MonadicBits;

namespace Pass.Components.Encryption
{
    public sealed class KeyRepository
    {
        public Maybe<IFile> PublicKey { get; }
        public Maybe<IFile> PrivateKey { get; }
    }
}