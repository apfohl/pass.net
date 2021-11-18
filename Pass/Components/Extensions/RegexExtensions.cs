using System.Text.RegularExpressions;
using MonadicBits;

namespace Pass.Components.Extensions;

using static Functional;

public static class RegexExtensions
{
    public static Maybe<Match> MatchInput(this Regex regex, string input)
    {
        var match = regex.Match(input);
        return match.Success ? match : Nothing;
    }
}