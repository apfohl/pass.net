using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MonadicBits;
using Pass.Components.Extensions;
using Pass.Components.FileSystem;
using Pass.Models;

namespace Pass.Components.Encryption;

public static class Decrypt
{
    public static Task<Maybe<Password>> DecryptedPassword(PasswordRepository passwordRepository,
        KeyRepository keyRepository, string name)
    {
        var stream = from file in passwordRepository.Find($"{name}.gpg")
            from keyStream in keyRepository.PrivateKey.Bind(keyFile => keyFile.OpenRead())
            from password in keyRepository.Password
            from decryptedStream in DecryptedStream(file, keyStream, password)
            select decryptedStream;

        return stream.BindAsync(async s =>
        {
            await using (s)
            {
                return await Password(name, s);
            }
        });
    }

    private static Maybe<Stream> DecryptedStream(IEncryptedFile file, Stream keyStream, string password)
    {
        using (keyStream)
        {
            return file.OpenRead(keyStream, password);
        }
    }

    private static async Task<Maybe<Password>> Password(string name, Stream stream)
    {
        using var reader = new StreamReader(stream);

        return await (await ReadPassword(reader))
            .MapAsync(async password =>
            {
                var metadata = await ReadLines(reader).AggregateAsync(new Dictionary<string, string>(),
                    (dictionary, line) =>
                    {
                        ParseLine(line).Match(tuple => dictionary.Add(tuple.key.ToLower(), tuple.value), () => { });
                        return dictionary;
                    });

                return new Password(name, password, metadata);
            });
    }

    private static async Task<Maybe<string>> ReadPassword(TextReader reader) =>
        (await reader.ReadLineAsync()).ToMaybe();

    private static Maybe<(string key, string value)> ParseLine(string line) =>
        new Regex("^([^:]*):(.+)$")
            .MatchInput(line)
            .Map(match => match.Groups)
            .Map(groups => (groups[1].Value, groups[2].Value));

    private static async IAsyncEnumerable<string> ReadLines(TextReader reader)
    {
        string line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            yield return line;
        }
    }
}