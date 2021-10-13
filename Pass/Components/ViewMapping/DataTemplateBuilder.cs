using System;
using Avalonia.Controls;

namespace Pass.Components.ViewMapping
{
    public static class DataTemplateBuilder
    {
        public static IControl BuildFromViewType(Type viewType) =>
            (IControl) Activator.CreateInstance(viewType);
    }
}