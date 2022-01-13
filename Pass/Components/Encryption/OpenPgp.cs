using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MonadicBits;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace Pass.Components.Encryption;

using static Functional;

public static class OpenPgp
{
    public static async Task Encrypt(
        Func<Stream, Task> writeAction,
        Stream outputStream,
        PgpPublicKey key,
        string fileName)
    {
        var pgpEncryptedDataGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, true);
        pgpEncryptedDataGenerator.AddMethod(key);
        await using var encryptedOutputStream = pgpEncryptedDataGenerator.Open(outputStream, new byte[1 << 8]);

        var pgpLiteralDataGenerator = new PgpLiteralDataGenerator();
        await using var literalStream = pgpLiteralDataGenerator.Open(encryptedOutputStream,
            PgpLiteralData.Binary, fileName, DateTime.Now, new byte[1 << 8]);

        await writeAction(literalStream);

        pgpLiteralDataGenerator.Close();
        pgpEncryptedDataGenerator.Close();
    }

    public static Maybe<PgpPublicKey> FindKey(this PgpPublicKeyRingBundle keyRing, string fingerprint) =>
        keyRing.GetKeyRings().Cast<PgpPublicKeyRing>()
            .SelectMany(ring => ring.GetPublicKeys().Cast<PgpPublicKey>())
            .Where(key => key.IsEncryptionKey)
            .Where(key => key.Fingerprint().EndsWith(fingerprint.ToUpper()))
            .FirstOrNothing();

    private static string Fingerprint(this PgpPublicKey key) =>
        BitConverter.ToString(key.GetFingerprint()).Replace("-", string.Empty).ToUpper();

    public static PgpPublicKeyRingBundle PublicKeyRingBundle(Stream keyStream) =>
        new(PgpUtilities.GetDecoderStream(keyStream));

    public static Maybe<Stream> Decrypt(Stream inputStream, Stream keyStream, string password) =>
        inputStream
            .DecoderStream()
            .EncryptedDataList()
            .Bind(edl => edl.DecryptedStream(keyStream, password))
            .Bind(LiteralDataStream);

    private static Stream DecoderStream(this Stream stream) =>
        PgpUtilities.GetDecoderStream(stream);

    private static Maybe<PgpEncryptedDataList> EncryptedDataList(this Stream stream)
    {
        var factory = stream.Factory();

        for (var pgpObject = factory.NextPgpObject(); pgpObject != null; pgpObject = factory.NextPgpObject())
        {
            if (pgpObject is PgpEncryptedDataList encryptedDataList)
            {
                return encryptedDataList;
            }
        }

        return Nothing;
    }

    private static Maybe<Stream> DecryptedStream(
        this PgpEncryptedDataList edl,
        Stream keyStream,
        string password) =>
        (from data in edl.GetEncryptedDataObjects().Cast<PgpPublicKeyEncryptedData>()
            from privateKey in
                SecretKeyRingBundle(keyStream)
                    .FindKey(data.KeyId)
                    .Bind(key => key.UnlockKey(password))
                    .ToEnumerable()
            select data.GetDataStream(privateKey))
        .FirstOrNothing();

    private static Maybe<Stream> LiteralDataStream(Stream stream)
    {
        using (stream)
        {
            return stream.Factory().NextPgpObject() is PgpLiteralData literalData
                ? literalData.GetInputStream()
                : Nothing;
        }
    }

    private static PgpObjectFactory Factory(this Stream stream) => new(stream);

    private static PgpSecretKeyRingBundle SecretKeyRingBundle(Stream keyStream) =>
        new(PgpUtilities.GetDecoderStream(keyStream));

    private static Maybe<PgpSecretKey> FindKey(this PgpSecretKeyRingBundle secretKeyRingBundle, long keyId) =>
        secretKeyRingBundle.GetSecretKey(keyId).JustNotNull();

    private static Maybe<PgpPrivateKey> UnlockKey(this PgpSecretKey key, string password) =>
        key.ExtractPrivateKey(password.ToCharArray()).JustNotNull();
}