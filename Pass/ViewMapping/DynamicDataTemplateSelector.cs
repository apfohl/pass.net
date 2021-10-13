using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using MonadicBits;
using Pass.Components;

namespace Pass.ViewMapping
{
    public sealed class DynamicDataTemplateSelector : IDataTemplate
    {
        public IControl Build(object item) =>
            TryFindViewAttribute(item).Match(
                attribute => DataTemplateBuilder.BuildFromViewType(attribute.View),
                () => throw new Exception("No DataTemplate found!"));

        public bool Match(object item) => TryFindViewAttribute(item).Match(_ => true, () => false);

        private static Maybe<ViewAttribute> TryFindViewAttribute(object item) =>
            item.JustNotNull()
                .Bind(i => i
                    .GetType()
                    .GetCustomAttributes(typeof(ViewAttribute), true)
                    .OfType<ViewAttribute>()
                    .ToArray()
                    .SingleOrNothing());
    }
}