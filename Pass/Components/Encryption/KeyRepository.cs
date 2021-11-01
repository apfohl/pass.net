using Bridgefield.PersistentBits.FileSystem;
using MonadicBits;

namespace Pass.Components.Encryption
{
    public sealed record KeyRepository(Maybe<IFile> PrivateKey, Maybe<IFile> PublicKey, Maybe<string> Password);
}