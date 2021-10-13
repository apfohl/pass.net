using System;

namespace Pass.ViewMapping
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal sealed class ViewAttribute : Attribute
    {
        public ViewAttribute(Type view) => View = view;

        public Type View { get; }
    }
}