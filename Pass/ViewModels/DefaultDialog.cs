using System;
using System.Collections.Generic;
using System.Linq;
using Pass.Components.Binding;
using Pass.Components.Dialog;
using Pass.Components.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels
{
    [View(typeof(DialogView))]
    public sealed class DefaultDialog: Bindable, IDialog
    {
        public string Title => "Default Dialog";
        public double MinWidth => 600;
        public double MinHeight => 400;
        public Bindable Content { get; }
        public IEnumerable<object> Buttons => Enumerable.Empty<object>();
        public event EventHandler Closed;

        public DefaultDialog(Bindable content) => Content = content;
    }
}