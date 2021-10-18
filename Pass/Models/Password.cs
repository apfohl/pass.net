using System.IO;
using System.Threading.Tasks;
using Bridgefield.PersistentBits.FileSystem;
using MonadicBits;

namespace Pass.Models
{
    public sealed class Password
    {
        public byte[] EncryptedData { get; }

        private Password(byte[] encryptedData) => EncryptedData = encryptedData;

        public static Task<Maybe<Password>> FromFile(IFile file) =>
            file.OpenRead()
                .MapAsync(async stream =>
                {
                    await using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                })
                .MapAsync(data => new Password(data));
    }
}