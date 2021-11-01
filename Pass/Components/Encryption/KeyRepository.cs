using Bridgefield.PersistentBits.FileSystem;
using MonadicBits;

namespace Pass.Components.Encryption
{
    public sealed class KeyRepository
    {
        public Maybe<IFile> PrivateKey { get; }
        public Maybe<IFile> PublicKey { get; }

        public KeyRepository(Maybe<IFile> privateKey, Maybe<IFile> publicKey)
        {
            PrivateKey = privateKey;
            PublicKey = publicKey;
        }
    }
}