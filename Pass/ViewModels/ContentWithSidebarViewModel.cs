using Pass.Components.Binding;
using Pass.Components.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels
{
    [View(typeof(ContentWithSidebarView))]
    public sealed class ContentWithSidebarViewModel : Bindable
    {
        public Bindable Content { get; }
        public Bindable Sidebar { get; }

        public ContentWithSidebarViewModel(Bindable content, Bindable sidebar)
        {
            Content = content;
            Sidebar = sidebar;
        }
    }
}