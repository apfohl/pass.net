using System;
using JetBrains.Annotations;

namespace Pass;

public sealed class FilePathConfiguration
{
    private readonly string rawValue;

    public FilePathConfiguration(string rawValue) => this.rawValue = rawValue;

    public string Path => Environment
        .ExpandEnvironmentVariables(rawValue)
        .Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
}

public sealed class Filesystem
{
    private string KeyStorage { get; [UsedImplicitly] set; }
    private string PasswordStore { get; [UsedImplicitly] set; }

    public FilePathConfiguration KeyStoragePath => new(KeyStorage);
    public FilePathConfiguration PasswordStorePath => new(PasswordStore);
}

public class AppSettings
{
    public Filesystem Filesystem { get; [UsedImplicitly] set; } = new();
}