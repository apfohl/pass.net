using System;
using System.IO;
using System.Threading.Tasks;
using MonadicBits;
using Pass.Components.Encryption;

namespace Pass.Components.FileSystem
{
    public sealed class EncryptedFile : IEncryptedFile
    {
        public string Path { get; }
        public string Name => System.IO.Path.GetFileName(Path);

        public EncryptedFile(string path) => Path = path;

        public bool Exists() => File.Exists(Path);

        public Maybe<Stream> OpenRead(Stream keyStream, string password) =>
            Path.JustWhen(File.Exists)
                .Bind(s =>
                {
                    using var inputStream = File.OpenRead(s);
                    return OpenPgp.Decrypt(inputStream, keyStream, password);
                });

        public async Task Write(Func<Stream, Task> writeAction, Stream keyStream, string fingerprint)
        {
            await using var outputStream = File.OpenWrite(Path);
            var keyRingBundle = OpenPgp.PublicKeyRingBundle(keyStream);

            await keyRingBundle
                .FindKey(fingerprint)
                .Match(
                    async key => { await OpenPgp.Encrypt(writeAction, outputStream, key, Name); },
                    () => Task.FromException(new Exception("No public key found!")));
        }
    }
}