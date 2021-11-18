using System.Collections.Generic;

namespace Pass.Models;

public sealed record Password(string Name, string Value, IDictionary<string, string> Metadata);