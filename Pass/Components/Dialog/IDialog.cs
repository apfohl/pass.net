using System;
using System.Collections.Generic;
using Pass.Components.Binding;

namespace Pass.Components.Dialog
{
    public interface IDialog
    {
        string Title { get; }
        public double MinWidth { get; }
        public double MinHeight { get; }
        public Bindable Content { get; }
        public IEnumerable<object> Buttons { get; }
        event EventHandler Closed;
    }
}