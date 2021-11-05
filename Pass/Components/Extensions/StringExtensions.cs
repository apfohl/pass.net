namespace Pass.Components.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveFromEnd(this string @this, string suffix) =>
            @this.EndsWith(suffix).OnTrue(() => @this[..^suffix.Length]).Match(s => s, () => @this);

        public static bool ContainsString(this string @this, string searchString) =>
            string.IsNullOrEmpty(searchString) || @this.Contains(searchString);
    }
}