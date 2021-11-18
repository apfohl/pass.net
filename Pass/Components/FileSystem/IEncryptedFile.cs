using System;
using System.IO;
using System.Threading.Tasks;
using Bridgefield.PersistentBits.FileSystem;
using MonadicBits;

namespace Pass.Components.FileSystem;

public interface IEncryptedFile : IFileSystemEntry
{
    bool Exists();

    Maybe<Stream> OpenRead(Stream keyStream, string password);

    Task Write(Func<Stream, Task> writeAction, Stream keyStream, string fingerprint);
}