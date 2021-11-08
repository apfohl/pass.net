using Avalonia;
using Avalonia.Controls;

namespace Pass.Components.Extensions
{
    public static class ResourceExtensions
    {
        private static T Resource<T>(this string key) where T : class =>
            Application.Current.FindResource(key) as T;
    }
}